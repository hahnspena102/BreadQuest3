using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class EnemyAnimator : MonoBehaviour
{
    [Header("Data")]
    [ReadOnly][SerializeField] private EnemyData enemyData;

    [Header("Animator Placeholders (from base controller)")]
    [SerializeField] private AnimationClip idleFPlaceholder;
    [SerializeField] private AnimationClip idleBPlaceholder;
    [SerializeField] private AnimationClip moveFPlaceholder;
    [SerializeField] private AnimationClip moveBPlaceholder;
    [SerializeField] private AnimationClip attackFPlaceholder;
    [SerializeField] private AnimationClip attackBPlaceholder;

    [Header("Debug")]
    [ReadOnly][SerializeField] private Animator animator;
    [ReadOnly][SerializeField] private AnimatorOverrideController overrideController;
    [ReadOnly][SerializeField] private RuntimeAnimatorController baseController;

    public EnemyData EnemyData
    {
        get => enemyData;
        set
        {
            enemyData = value;
            ApplyEnemyData();
        }
    }

    public AnimationClip IdleFPlaceholder { get => idleFPlaceholder; set => idleFPlaceholder = value; }
    public AnimationClip IdleBPlaceholder { get => idleBPlaceholder; set => idleBPlaceholder = value; }
    public AnimationClip MoveFPlaceholder { get => moveFPlaceholder; set => moveFPlaceholder = value; }
    public AnimationClip MoveBPlaceholder { get => moveBPlaceholder; set => moveBPlaceholder = value; }
    public AnimationClip AttackFPlaceholder { get => attackFPlaceholder; set => attackFPlaceholder = value; }
    public AnimationClip AttackBPlaceholder { get => attackBPlaceholder; set => attackBPlaceholder = value; }

    void Awake()
    {
        animator = GetComponent<Animator>();

        baseController = animator.runtimeAnimatorController;
        overrideController = new AnimatorOverrideController(baseController);
        animator.runtimeAnimatorController = overrideController;
        
        StartCoroutine(ApplyEnemyData());
    }

    void ApplyOverride(AnimationClip placeholder, AnimationClip newClip)
    {
        if (!placeholder) return;

        if (newClip)
            overrideController[placeholder.name] = newClip;
        else
            overrideController[placeholder.name] = placeholder;
    }

    IEnumerator ApplyEnemyData()
    {
        while (enemyData == null || overrideController == null || animator == null)
            yield return null;

            
        if (EnemyData.AnimatorOverride)
        {
            animator.runtimeAnimatorController = EnemyData.AnimatorOverride;
            yield break;
        }


        ApplyOverride(idleFPlaceholder, enemyData.IdleAnimationF);
        ApplyOverride(idleBPlaceholder, enemyData.IdleAnimationB);
        ApplyOverride(moveFPlaceholder, enemyData.MoveAnimationF);
        ApplyOverride(moveBPlaceholder, enemyData.MoveAnimationB);
        ApplyOverride(attackFPlaceholder, enemyData.AttackAnimationF);
        ApplyOverride(attackBPlaceholder, enemyData.AttackAnimationB);

        animator.Rebind();
        animator.Update(0f);
        animator.Play("EnemyIdleF", 0, 0f);
    }

    
}
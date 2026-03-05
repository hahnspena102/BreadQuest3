using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class EnemyAnimator : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemyData enemyData;

    [Header("Animator Placeholders (from base controller)")]
    [SerializeField] private AnimationClip idleFPlaceholder;
    [SerializeField] private AnimationClip idleBPlaceholder;

    [SerializeField] private Animator animator;
    [SerializeField] private AnimatorOverrideController overrideController;
    [SerializeField] private RuntimeAnimatorController baseController;

    public EnemyData EnemyData
    {
        get => enemyData;
        set
        {
            enemyData = value;
            ApplyEnemyData();
        }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();

        baseController = animator.runtimeAnimatorController;
        overrideController = new AnimatorOverrideController(baseController);
        animator.runtimeAnimatorController = overrideController;
        
        StartCoroutine(ApplyEnemyData());
    }

    
    IEnumerator ApplyEnemyData()
    {
        while (enemyData == null || overrideController == null || animator == null) 
            yield return null;

        if (idleFPlaceholder && enemyData.IdleAnimationF)
            overrideController[idleFPlaceholder] = enemyData.IdleAnimationF;

        if (idleBPlaceholder && enemyData.IdleAnimationB)
            overrideController[idleBPlaceholder] = enemyData.IdleAnimationB;

        animator.Rebind();
        animator.Update(0f);
        animator.Play("EnemyIdleF", 0, 0f);
    }

}
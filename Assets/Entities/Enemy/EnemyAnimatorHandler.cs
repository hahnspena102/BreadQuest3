using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class EnemyAnimatorHandler : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemyData enemyData;

    [Header("Animator Placeholders (from base controller)")]
    [SerializeField] private AnimationClip idleFPlaceholder;
    [SerializeField] private AnimationClip idleBPlaceholder;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AnimatorOverrideController overrideController;

    void Awake()
    {
        CacheComponents();
        SetupOverrideController();
        ApplyEnemyData();
    }

    void OnEnable()
    {
        if (!Application.isPlaying)
            ApplyEnemyData();
    }

    void OnValidate()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall += () =>
        {
            if (this != null)
                ApplyEnemyData();
        };
#endif
    }

    void CacheComponents()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void SetupOverrideController()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return;

        if (overrideController == null)
        {
            overrideController =
                new AnimatorOverrideController(animator.runtimeAnimatorController);

            animator.runtimeAnimatorController = overrideController;
        }
    }

    public void ApplyEnemyData()
    {
        if (enemyData == null)
            return;

        CacheComponents();
        SetupOverrideController();

        // Sprite
        spriteRenderer.sprite = enemyData.EnemySprite;

        // Animation overrides
        if (idleFPlaceholder && enemyData.IdleAnimationF)
            overrideController[idleFPlaceholder] = enemyData.IdleAnimationF;

        if (idleBPlaceholder && enemyData.IdleAnimationB)
            overrideController[idleBPlaceholder] = enemyData.IdleAnimationB;

        animator.Rebind();
        animator.Update(0f);
    }
}

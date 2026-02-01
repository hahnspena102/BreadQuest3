using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer), typeof(Animator), typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemyData enemyData;

    [Header("Animator Placeholders (from base controller)")]
    [SerializeField] private AnimationClip idleFPlaceholder;
    [SerializeField] private AnimationClip idleBPlaceholder;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;

    private AnimatorOverrideController overrideController;

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
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        SetupOverrideController();
        ApplyEnemyData();
    }

    void Start()
    {
        StartCoroutine(BehaviorLoop());
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.x > 0.01f)
            transform.localScale = Vector3.one;
        else if (rb.linearVelocity.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void OnValidate()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall += ApplyEnemyData;
#endif
    }

    // ---------------- CORE LOGIC ----------------

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

    void ApplyEnemyData()
    {
        if (this == null || enemyData == null)
            return;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
            animator = GetComponent<Animator>();

        SetupOverrideController();

        // Sprite
        spriteRenderer.sprite = enemyData.EnemySprite;

        // Animations 
        if (idleFPlaceholder && enemyData.IdleAnimationF)
            overrideController[idleFPlaceholder] = enemyData.IdleAnimationF;

        if (idleBPlaceholder && enemyData.IdleAnimationB)
            overrideController[idleBPlaceholder] = enemyData.IdleAnimationB;

        // Force animator refresh
        animator.Rebind();
        animator.Update(0f);
    }

    IEnumerator BehaviorLoop()
    {
        if (enemyData == null || enemyData.EnemyBehaviors.Count == 0)
            yield break;

        while (true)
        {
            EnemyBehavior eb =
                enemyData.EnemyBehaviors[Random.Range(0, enemyData.EnemyBehaviors.Count)];

            eb.PerformBehavior(gameObject);
            yield return new WaitForSeconds(0.5f);
        }
    }
}

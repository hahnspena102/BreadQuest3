using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemyData enemyData;
    public Wave AssignedWave { get; set; }
    public Room AssignedRoom { get; set; }
    [SerializeField] private float currentHealth, maxHealth;
    [SerializeField] private float contactDamage;
    
    [SerializeField] private Transform shadowTransform;
    [SerializeField]private PhysicsMaterial2D bouncyMaterial;


    [Header("Combat")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private bool isAttackReady = false;

    [ReadOnly][SerializeField] private PopupManager popupManager;

    [SerializeField] private Enemy linkedEnemy;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private UnityEngine.AI.NavMeshAgent agent;
    private Coroutine behaviorCoroutine;
    private Player player;




    public GameObject ProjectilePrefab { get => projectilePrefab; set => projectilePrefab = value; }
    public EnemyData EnemyData { get => enemyData; set => enemyData = value; }
    public Transform ShadowTransform { get => shadowTransform; set => shadowTransform = value; }
    public global::System.Boolean IsAttackReady { get => isAttackReady; set => isAttackReady = value; }
    public Enemy LinkedEnemy { get => linkedEnemy; set => linkedEnemy = value; }
    public Player Player { get => player; set => player = value; }
    public global::System.Single ContactDamage { get => contactDamage; set => contactDamage = value; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        popupManager = FindFirstObjectByType<PopupManager>();
        player = FindFirstObjectByType<Player>();
    }

    void Start()
    {
        ApplyEnemyData();
        //agent.enabled = true;
        behaviorCoroutine = StartCoroutine(BehaviorLoop());
        float floor = player != null ? player.PlayerData.CurrentFloor : 0f;
        currentHealth = enemyData.MaxHealth + (enemyData.MaxHealth * enemyData.HealthScalar) * floor;
        maxHealth = currentHealth;
        contactDamage = enemyData.ContactDamage + (enemyData.ContactDamage * enemyData.DamageScalar) * floor;
        if (enemyData.IgnoreEnemyCollision)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer);
        }
    }

    void OnDisable()
    {
        if (behaviorCoroutine != null)
        {
            StopCoroutine(behaviorCoroutine);
            behaviorCoroutine = null;
        }
    }

    void Update()
    {
        if (agent.velocity.x > 0.01f)
            transform.localScale = Vector3.one;
        else if (agent.velocity.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        if (animator != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("speed", speed);
        } else
        {
            Debugger.LogWarning("Enemy " + gameObject.name + " has no Animator component for animation control.", context: this, type: DebugType.World);
        }    
    }

    IEnumerator BehaviorLoop()
    {
        if (enemyData == null)
            yield break;

        // Execute on-spawn behaviors first
        foreach (EnemyBehaviorEntry spawnBehavior in enemyData.OnSpawnBehaviors)
        {
            yield return ExecuteBehavior(spawnBehavior);
        }

        if (enemyData.Behaviors.Count == 0)
            yield break;

        while (true)
        {
            if (enemyData.PerformBehaviorsInOrder)
            {
                foreach (EnemyBehaviorEntry behaviorEntry in enemyData.Behaviors)
                {
                    yield return ExecuteBehavior(behaviorEntry);
                }
            }
            else
            {
                EnemyBehaviorEntry randomEntry =
                    enemyData.Behaviors[Random.Range(0, enemyData.Behaviors.Count)];

                yield return ExecuteBehavior(randomEntry);
            }
        }
    }

    private IEnumerator ExecuteBehavior(EnemyBehaviorEntry behaviorEntry)
    {
        EnemyBehavior behavior = behaviorEntry.Behavior;

        for (int i = 0; i < behaviorEntry.TimesToRepeat; i++)
        {
            Debugger.Log($"Enemy {gameObject.name} performing behavior {behavior.name} (iteration {i + 1}/{behaviorEntry.TimesToRepeat})", context: this, type: DebugType.World);
            float timeToWait = behavior.PerformBehavior(
                this,
                behaviorEntry.BehaviorDuration
            );

            yield return new WaitForSeconds(timeToWait);
        }
    }

    public void TakeDamage(float damage, Flavor flavor = null)
    {
        float totalDamage = damage;
        Flavor effectiveAgainst = flavor?.EffectiveAgainst;
        bool isEffective = effectiveAgainst != null && enemyData.Flavor != null && effectiveAgainst == enemyData.Flavor;
      
        if (effectiveAgainst == enemyData.Flavor)
        {
        
            totalDamage = damage * 1.5f;
        }
     
        currentHealth -= totalDamage;


        if (popupManager != null)
        {
            Color? outlineColor = flavor != null ? flavor.FlavorColor : (Color?)null;
//            Debug.Log("Passing outline color: " + (outlineColor.HasValue ? outlineColor.Value.ToString() : "None"));
            popupManager.ShowDamagePopup(transform.position, (int)totalDamage, isEffective, false, outlineColor);
        }
        StartCoroutine(DamageFlash());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageFlash()
    {
        Color startColor = Color.red;
        Color endColor = Color.white;

        float duration = 0.3f; // total fade time
        float elapsed = 0f;

        sr.color = startColor;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            sr.color = Color.Lerp(startColor, endColor, elapsed / duration);
            yield return null;
        }

        sr.color = endColor;
    }

    public void Die()
    {
        if (popupManager != null)
        {
            popupManager.ShowDeathParticles(transform.position);
        }
        if (AssignedWave != null)
        {
            AssignedWave.enemiesLeft--;
            Debugger.Log("Remaining enemies in wave: " + AssignedWave.enemiesLeft, context: this, type: DebugType.World);
        } else
        {
            Debugger.LogWarning("Enemy " + gameObject.name + " has no assigned wave to remove itself from.", context: this, type: DebugType.World);
        }

        if (linkedEnemy != null)
        {
            linkedEnemy.LinkedEnemy = null;
            linkedEnemy.TakeDamage(linkedEnemy.currentHealth);
            linkedEnemy.SetLinkedEnemy(null);
        }

        player.PlayerData.Experience += enemyData.ExperienceDropped;

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            Player player = collision.gameObject.GetComponentInParent<Player>();
            
            if (player != null)
            {
                Weapon weapon = player.Inventory.EquippedItem as Weapon;
                WeaponData weaponData = weapon != null ? weapon.WeaponData : null;
                TakeDamage(weaponData != null ? weaponData.Damage : 0f, weaponData != null ? weaponData.Flavor : null);

            }
        }
    }



    public void ApplyEnemyData()
    {
        if (enemyData == null)
            return;

        if (enemyData.IsBouncy && bouncyMaterial != null)
        {
            rb.sharedMaterial = bouncyMaterial;
        }


        Pathfinder pathfinder = GetComponent<Pathfinder>();
        if (pathfinder != null)
        {
            pathfinder.EnemyData = enemyData;
            UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null && !enemyData.DisableAgent)
            {
                agent.enabled = true;
                agent.updateRotation = false;
                agent.updateUpAxis = false;
                agent.speed = enemyData.Speed;
            }
        }

        EnemyAnimator anim = GetComponent<EnemyAnimator>();
        if (anim != null) 
            anim.EnemyData = enemyData;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && enemyData.EnemySprite)
            sr.sprite = enemyData.EnemySprite;

        ShadowTransform.localScale = Vector3.one * enemyData.ShadowScale;
        ShadowTransform.localPosition = enemyData.ShadowOffset;
    }

    public void AttackReady()
    {
        IsAttackReady = true;
    }

    public void SetLinkedEnemy(Enemy enemy)
    {
        linkedEnemy = enemy;
    }
}

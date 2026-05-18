using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemyData enemyData;
    public Wave AssignedWave { get; set; }
    public Room AssignedRoom { get; set; }
    [SerializeField] private float currentHealth, maxHealth;
    [SerializeField] private float contactDamage;
    [SerializeField] private float currentDefense;
    private float attackDamage;
    
    [SerializeField] private Transform shadowTransform;
    [SerializeField]private PhysicsMaterial2D bouncyMaterial;


    [Header("Combat")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private bool isAttackReady = false;
    private float repeatedHitCooldown = 0.2f;

    [ReadOnly][SerializeField] private PopupManager popupManager;

    [SerializeField] private Enemy linkedEnemy;
    [SerializeField] private bool isBoss = false;
    [SerializeField] private float scaleFactor = 1f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private UnityEngine.AI.NavMeshAgent agent;
    private Coroutine behaviorCoroutine;
    private Player player;
    private readonly Dictionary<int, float> lastHitTimesBySource = new Dictionary<int, float>();
    




    public GameObject ProjectilePrefab { get => projectilePrefab; set => projectilePrefab = value; }
    public EnemyData EnemyData { get => enemyData; set => enemyData = value; }
    public Transform ShadowTransform { get => shadowTransform; set => shadowTransform = value; }
    public global::System.Boolean IsAttackReady { get => isAttackReady; set => isAttackReady = value; }
    public Enemy LinkedEnemy { get => linkedEnemy; set => linkedEnemy = value; }
    public Player Player { get => player; set => player = value; }
    public global::System.Single ContactDamage { get => contactDamage; set => contactDamage = value; }
    public global::System.Single AttackDamage { get => attackDamage; set => attackDamage = value; }
    public global::System.Single ScaleFactor { get => scaleFactor; set => scaleFactor = value; }
    public global::System.Boolean IsBoss { get => isBoss; set => isBoss = value; }
    public global::System.Single CurrentDefense { get => currentDefense; set => currentDefense = value; }

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
        Debugger.Log("Base health: " + enemyData.MaxHealth + ", scaled health: " + GameManager.CalculateValueByFloor(enemyData.MaxHealth, enemyData.HealthScalar, (int)floor)            + ", base damage: " + enemyData.ContactDamage + ", scaled damage: " + GameManager.CalculateValueByFloor(enemyData.ContactDamage, enemyData.DamageScalar, (int)floor) , context: this, type: DebugType.Enemies);
        currentHealth = GameManager.CalculateValueByFloor(enemyData.MaxHealth, enemyData.HealthScalar, (int)floor);
        maxHealth = currentHealth;
        currentDefense = enemyData.Defense;
        contactDamage = GameManager.CalculateValueByFloor(enemyData.ContactDamage, enemyData.DamageScalar, (int)floor);
        attackDamage = GameManager.CalculateValueByFloor(enemyData.BaseDamage, enemyData.DamageScalar, (int)floor);
        if (enemyData.IgnoreEnemyCollision)
        {
            //ignore other enemies
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);
        }
        if (enemyData.IsDynamic)
        {
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        if (isBoss)
        {
            currentHealth *= 5f;
            maxHealth = currentHealth;
            contactDamage *= 1.5f;
            attackDamage *= 1.5f;
            scaleFactor = 2f;
            transform.localScale = Vector3.one * scaleFactor;
             Debugger.Log("Boss enemy stats - health: " + currentHealth + ", contact damage: " + contactDamage + ", attack damage: " + attackDamage, context: this, type: DebugType.Enemies);
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
            transform.localScale = Vector3.one * scaleFactor;
        else if (agent.velocity.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1) * scaleFactor;

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
            Debugger.Log($"Enemy {gameObject.name} performing behavior {behavior.name} (iteration {i + 1}/{behaviorEntry.TimesToRepeat})", context: this, type: DebugType.AI);
            float timeToWait = behavior.PerformBehavior(
                this,
                behaviorEntry.BehaviorDuration
            );

            yield return new WaitForSeconds(timeToWait);
        }
    }

    public void TakeDamage(float damage, Flavor flavor = null, bool isTrueDamage = false)
    {
        if (currentHealth <= 0)
            return;
        float totalDamage = damage;
        Flavor effectiveAgainst = flavor?.EffectiveAgainst;
        bool isEffective = effectiveAgainst != null && enemyData.Flavor != null && effectiveAgainst == enemyData.Flavor;

        if (!isTrueDamage)
        {
            totalDamage *= 1f - currentDefense;
        }

        if (effectiveAgainst == enemyData.Flavor)
        {
        
            totalDamage = totalDamage * 1.5f;
        }

        totalDamage = totalDamage * (100f / (100f + enemyData.Defense));
     
        currentHealth -= totalDamage;

        SoundManager.instance.PlaySoundFXClip(enemyData.GetHurtSound(), transform);


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
        SoundManager.instance.PlaySoundFXClip(enemyData.GetDeathSound(), transform);
        if (AssignedWave != null)
        {
            AssignedWave.enemiesLeft--;
        }

        if (linkedEnemy != null)
        {
            linkedEnemy.LinkedEnemy = null;
            linkedEnemy.TakeDamage(linkedEnemy.currentHealth, null, true);
            linkedEnemy.SetLinkedEnemy(null);
        }

        player.PlayerData.Experience += enemyData.ExperienceDropped;

        Destroy(gameObject, 0.01f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            if (!CanTakeDamageFrom(collision.gameObject))
            {
                return;
            }

            Player player = collision.gameObject.GetComponentInParent<Player>();
            
            if (player != null)
            {
                Weapon weapon = player.Inventory.EquippedItem as Weapon;
                WeaponData weaponData = weapon != null ? weapon.WeaponData : null;
                TakeDamage(weaponData != null ? weaponData.Damage : 0f, weaponData != null ? weaponData.Flavor : null);

            }
        }
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            if (!CanTakeDamageFrom(collision.gameObject))
            {
                return;
            }

            if (player != null)
            {
                Projectile projectile = collision.gameObject.GetComponent<Projectile>();
                if (projectile != null)
                {
                    TakeDamage(projectile.ProjectileDamage, projectile.Flavor);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            if (!CanTakeDamageFrom(collision.gameObject))
            {
                return;
            }

            if (player != null)
            {
                Projectile projectile = collision.gameObject.GetComponent<Projectile>();
                if (projectile != null)
                {
                    TakeDamage(projectile.ProjectileDamage, projectile.Flavor);
                }
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

    private bool CanTakeDamageFrom(GameObject source)
    {
        if (source == null)
        {
            return false;
        }

        int sourceId = source.GetInstanceID();
        if (lastHitTimesBySource.TryGetValue(sourceId, out float lastHitTime))
        {
            if (Time.time - lastHitTime < repeatedHitCooldown)
            {
                return false;
            }
        }

        lastHitTimesBySource[sourceId] = Time.time;
        return true;
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}

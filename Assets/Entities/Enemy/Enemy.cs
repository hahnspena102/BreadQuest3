using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Room assignedRoom;
    [SerializeField] private float currentHealth;

    [Header("Combat")]
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private PopupManager popupManager;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private UnityEngine.AI.NavMeshAgent agent;
    private Coroutine behaviorCoroutine;


    public GameObject ProjectilePrefab { get => projectilePrefab; set => projectilePrefab = value; }
    public EnemyData EnemyData { get => enemyData; set => enemyData = value; }
    public Room AssignedRoom { get => assignedRoom; set => assignedRoom = value; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        sr = GetComponent<SpriteRenderer>();
        popupManager = FindFirstObjectByType<PopupManager>();
    }

    void Start()
    {
        behaviorCoroutine = StartCoroutine(BehaviorLoop());
        currentHealth = enemyData.MaxHealth;
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

    }

    IEnumerator BehaviorLoop()
    {
        if (enemyData == null || enemyData.EnemyBehaviors.Count == 0)
            yield break;

        while (true)
        {
            if (enemyData.PerformBehaviorsInOrder)
            {
                foreach (EnemyBehavior behavior in enemyData.EnemyBehaviors)
                {
                    float timeToWait = behavior.PerformBehavior(this);
                    yield return new WaitForSeconds(timeToWait);
                }
            }
            else
            {
            EnemyBehavior behavior =
                enemyData.EnemyBehaviors[
                    Random.Range(0, enemyData.EnemyBehaviors.Count)
                ];

            float timeToWait = behavior.PerformBehavior(this);

            yield return new WaitForSeconds(timeToWait);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
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
        if (AssignedRoom != null)
        {
            AssignedRoom.enemiesInRoom.Remove(gameObject);
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            Player player = collision.gameObject.GetComponentInParent<Player>();
            
            if (player != null)
            {
                WeaponData weaponData = player.Inventory.EquippedItemData as WeaponData;
                TakeDamage(weaponData != null ? weaponData.Damage : 0f);
                if (popupManager != null)
                {
                    //bool isCritical = Random.value < player.PlayerData.CriticalHitChance;
                    int damageAmount = weaponData != null ? Mathf.RoundToInt(weaponData.Damage) : 0;
        
       
                    popupManager.ShowDamagePopup(transform.position, damageAmount, false, false);
                }
            }
        }
    }

}

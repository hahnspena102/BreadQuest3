using UnityEngine;

public enum ProjectileType
{
    Enemy,
    Player
}

public class Projectile : MonoBehaviour
{
    [ReadOnly, SerializeField]private ProjectileData projectileData;
    [ReadOnly, SerializeField]private float projectileDamage;
    [ReadOnly, SerializeField]private Flavor flavor;
    [SerializeField]private ProjectileType projectileType;
    [SerializeField]private int bouncesRemaining = 3;
    [SerializeField]private Animator animator;
    private float spawnGracePeriod = 0.01f; // seconds to ignore environment collisions after spawn
    private float spawnTime;
    private Collider2D projectileCollider;
    private bool originalColliderIsTrigger;
    

    public ProjectileData ProjectileData { get => projectileData; set => projectileData = value; }
    public global::System.Single ProjectileDamage { get => projectileDamage; set => projectileDamage = value; }
    public Flavor Flavor { get => flavor; set => flavor = value; }

    public void InitializeEnemyProjectile(Vector2 direction, Enemy enemy)
    {
        projectileType = ProjectileType.Enemy;
        gameObject.tag = "EnemyProjectile";
        //gameObject.layer = LayerMask.NameToLayer("Enemy");
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.excludeLayers = LayerMask.GetMask("Enemy") | LayerMask.GetMask("Projectile");

        

        projectileData = enemy.EnemyData.ProjectileData;
        if (projectileData == null)
        {
            Debug.LogError("ProjectileData is not assigned in EnemyData.");
            return;
        }
        flavor = enemy.EnemyData.Flavor;

        Collider2D enemyCollider = GetComponent<Collider2D>();
        if (enemyCollider != null && projectileData.IsTrigger)
        {
            enemyCollider.isTrigger = true;
        }

        InitializeProjectile(direction, projectileData);

        float floor = enemy.Player != null ? enemy.Player.PlayerData.CurrentFloor : 0f;
        float baseDamage = enemy.EnemyData.BaseDamage;
        projectileDamage = baseDamage + (baseDamage * enemy.EnemyData.DamageScalar) * floor;
        
    }

    public void InitializePlayerProjectile(Vector2 direction, ProjectileData data, Player player)
    {
        projectileType = ProjectileType.Player;
        gameObject.tag = "PlayerProjectile";
        //gameObject.layer = LayerMask.NameToLayer("Player");
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.excludeLayers = LayerMask.GetMask("Player") | LayerMask.GetMask("Projectile");
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null && data.IsTrigger)
        {
           collider.isTrigger = true;
        }

        Item equippedItem = player.Inventory.GetItemAtIndex(player.Inventory.CurrentItemIndex);
        WeaponData weaponData = equippedItem != null ? equippedItem.ItemData as WeaponData : null;
        flavor = weaponData != null ? weaponData.Flavor : null;
        InitializeProjectile(direction, data);
        float baseDamage = weaponData != null ? weaponData.Damage : 0f;

        projectileDamage = baseDamage;
    }

    public void InitializeProjectile(Vector2 direction, ProjectileData data)
    {

        projectileData = data;
        if (projectileData == null)
        {
            Debug.LogError("ProjectileData is not assigned.");
            return;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null && projectileData != null)
        {
            rb.linearVelocity = direction.normalized * projectileData.Speed;
            rb.angularVelocity = projectileData.RotationSpeed;
        }
        Destroy(gameObject, projectileData.Lifetime);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = projectileData.ProjectileSprite;
        }

        if (projectileData.RotateTowardsMovementDirection && rb != null)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (animator != null && projectileData.MovingAnimation != null)
        {
            OverrideMovingAnimation(projectileData.MovingAnimation);
        }

        // record spawn time for grace-period collision handling
        spawnTime = Time.time;

        // Temporarily make collider a trigger to avoid physics collisions with walls immediately after spawn.
        projectileCollider = GetComponent<Collider2D>();
        if (projectileCollider != null)
        {
            originalColliderIsTrigger = projectileCollider.isTrigger;
            if (!projectileData.IsTrigger && !projectileCollider.isTrigger)
            {
                projectileCollider.isTrigger = true;
                StartCoroutine(RestoreColliderAfterGrace(spawnGracePeriod));
            }
        }
        
    }

    private void OverrideMovingAnimation(AnimationClip movingClip)
    {
        if (animator == null || movingClip == null)
        {
            return;
        }

        RuntimeAnimatorController baseController = animator.runtimeAnimatorController;
        if (baseController == null)
        {
            return;
        }

        AnimatorOverrideController overrideController = new AnimatorOverrideController(baseController);
        AnimationClip[] clips = baseController.animationClips;
        if (clips == null || clips.Length == 0)
        {
            return;
        }
        
        for (int i = 0; i < clips.Length; i++)
        {
            overrideController[clips[i].name] = movingClip;
        }

        animator.runtimeAnimatorController = overrideController;
        animator.Play(0, 0, 0f);
        StartCoroutine(ResetPolygonColliderAfterAnimationUpdate());
    }

    private System.Collections.IEnumerator ResetPolygonColliderAfterAnimationUpdate()
    {
        yield return null;

        PolygonCollider2D polygonCollider = GetComponent<PolygonCollider2D>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (polygonCollider == null || spriteRenderer == null || spriteRenderer.sprite == null)
        {
            yield break;
        }

        Sprite sprite = spriteRenderer.sprite;
        int shapeCount = sprite.GetPhysicsShapeCount();
        if (shapeCount <= 0)
        {
            polygonCollider.pathCount = 0;
            yield break;
        }

        polygonCollider.pathCount = shapeCount;
        var path = new System.Collections.Generic.List<Vector2>();
        for (int i = 0; i < shapeCount; i++)
        {
            path.Clear();
            sprite.GetPhysicsShape(i, path);
            polygonCollider.SetPath(i, path);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (projectileType == ProjectileType.Enemy && other.CompareTag("Player"))
        {
           Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // During the initial spawn grace period, ignore collisions with non-character, non-projectile objects
        if (Time.time - spawnTime < spawnGracePeriod)
        {
            GameObject other = collision.gameObject;
            bool isPlayer = other.CompareTag("Player");
            bool isEnemy = other.CompareTag("Enemy");
            bool isProj = IsProjectileObject(other);

            if (!isPlayer && !isEnemy && !isProj)
            {
                // treat as environment/wall collider - ignore during grace period
                return;
            }
        }

        if (projectileType == ProjectileType.Player && !collision.gameObject.CompareTag("Player"))
        {
            if (projectileData.MaxBounces > 0)
            {
                bouncesRemaining--;
                if (bouncesRemaining <= 0)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        if (projectileType == ProjectileType.Enemy)
        {
           if (projectileData.MaxBounces > 0)
            {
                bouncesRemaining--;
                if (bouncesRemaining <= 0)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        
    }

    private System.Collections.IEnumerator RestoreColliderAfterGrace(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (projectileCollider == null)
            yield break;

        // Only restore if the projectileData doesn't require a trigger collider
        if (projectileData != null && !projectileData.IsTrigger)
        {
            projectileCollider.isTrigger = originalColliderIsTrigger;
            // After restoring, check if we're overlapping an environment (wall) collider.
            float checkRadius = 0.1f;
            try
            {
                if (projectileCollider.bounds.size.magnitude > 0f)
                {
                    checkRadius = Mathf.Max(0.05f, projectileCollider.bounds.extents.magnitude);
                }
            }
            catch {}

            Collider2D[] overlaps = Physics2D.OverlapCircleAll(transform.position, checkRadius);
            foreach (var col in overlaps)
            {
                if (col == null || col == projectileCollider) continue;
                if (col.isTrigger) continue;
                GameObject other = col.gameObject;
                bool isPlayer = other.CompareTag("Player");
                bool isEnemy = other.CompareTag("Enemy");
                bool isProj = IsProjectileObject(other);

                if (!isPlayer && !isEnemy && !isProj)
                {
                    // overlapping a wall/environment — destroy immediately to avoid bounce spam
                    Destroy(gameObject);
                    yield break;
                }
            }
        }
    }

    private bool IsProjectileObject(GameObject obj)
    {
        if (obj == null)
        {
            return false;
        }

        return obj.CompareTag("PlayerProjectile")
            || obj.CompareTag("EnemyProjectile")
            || obj.GetComponent<Projectile>() != null;
    }

    
}

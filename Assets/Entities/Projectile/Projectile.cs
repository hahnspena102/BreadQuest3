using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    Enemy,
    Player
}

public class Projectile : MonoBehaviour
{
    [ReadOnly, SerializeField] private ProjectileData projectileData;
    [ReadOnly, SerializeField] private float projectileDamage;
    [ReadOnly, SerializeField] private Flavor flavor;

    private Rigidbody2D rb;

    [SerializeField] private ProjectileType projectileType;
    [SerializeField] private int bouncesRemaining = 3;
    [SerializeField] private Animator animator;

    private float spawnGracePeriod = 0.02f;
    private float spawnTime;

    private Collider2D projectileCollider;
    private bool originalColliderIsTrigger;

    public ProjectileData ProjectileData { get => projectileData; set => projectileData = value; }
    public float ProjectileDamage { get => projectileDamage; set => projectileDamage = value; }
    public Flavor Flavor { get => flavor; set => flavor = value; }

    // =========================
    // INITIALIZATION (ENEMY)
    // =========================
    public void InitializeEnemyProjectile(Vector2 direction, Enemy enemy)
    {
        projectileType = ProjectileType.Enemy;
        gameObject.tag = "EnemyProjectile";

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.excludeLayers = LayerMask.GetMask("Enemy") | LayerMask.GetMask("Projectile");

        projectileData = enemy.EnemyData.ProjectileData;

        if (projectileData == null)
        {
            Debug.LogError("[Projectile] Enemy ProjectileData missing!");
            return;
        }

        flavor = enemy.EnemyData.Flavor;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null && projectileData.IsTrigger)
            col.isTrigger = true;

        InitializeProjectile(direction, projectileData);

        projectileDamage = enemy.AttackDamage;
    }

    // =========================
    // INITIALIZATION (PLAYER)
    // =========================
    public void InitializePlayerProjectile(Vector2 direction, ProjectileData data, Player player)
    {
        projectileType = ProjectileType.Player;
        gameObject.tag = "PlayerProjectile";

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.excludeLayers = LayerMask.GetMask("Player") | LayerMask.GetMask("Projectile");

        projectileData = data;

        if (projectileData == null)
        {
            Debug.LogError("[Projectile] Player ProjectileData missing!");
            return;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null && projectileData.IsTrigger)
            col.isTrigger = true;

        Item equippedItem = player.Inventory.GetItemAtIndex(player.Inventory.CurrentItemIndex);
        WeaponData weaponData = equippedItem != null ? equippedItem.ItemData as WeaponData : null;

        flavor = weaponData != null ? weaponData.Flavor : null;

        InitializeProjectile(direction, data);

        projectileDamage = weaponData != null ? weaponData.Damage : 0f;
    }

  
    public void InitializeProjectile(Vector2 direction, ProjectileData data)
    {
        projectileData = data;

        if (projectileData == null)
        {
            Debug.LogError("[Projectile] ProjectileData is NULL");
            return;
        }

        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * projectileData.Speed;
            rb.angularVelocity = projectileData.RotationSpeed;
        }

        Destroy(gameObject, projectileData.Lifetime);

        // Sprite setup (optional)
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && projectileData.ProjectileSprite != null)
        {
            sr.sprite = projectileData.ProjectileSprite;
            Debug.Log($"[Projectile] Sprite set: {sr.sprite.name}");
        }

        // Rotation
        if (projectileData.RotateTowardsMovementDirection && rb != null)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Animation (optional ONLY visual)
        if (animator != null && projectileData.MovingAnimation != null)
        {
            OverrideMovingAnimation(projectileData.MovingAnimation);
        }

        // IMPORTANT: physics setup ALWAYS happens independently
        TrySetupPhysicsShape();

        spawnTime = Time.time;

        // Scale
        if (projectileData.Scale != 1f)
        {
            transform.localScale = Vector3.one * projectileData.Scale;
        }
    }


    private void TrySetupPhysicsShape()
    {
        var sr = GetComponent<SpriteRenderer>();
        var poly = GetComponent<PolygonCollider2D>();

        if (!poly)
        {
            return;
        }

        if (!sr || sr.sprite == null)
        {
            return;
        }

        int shapeCount = sr.sprite.GetPhysicsShapeCount();

        if (shapeCount <= 0)
        {
            return;
        }

        poly.enabled = false;
        poly.pathCount = 0;
        poly.enabled = true;

        var path = new List<Vector2>();
        poly.pathCount = shapeCount;

        for (int i = 0; i < shapeCount; i++)
        {
            path.Clear();
            sr.sprite.GetPhysicsShape(i, path);
            poly.SetPath(i, path);

        }

    }

    private void OverrideMovingAnimation(AnimationClip movingClip)
    {
        RuntimeAnimatorController baseController = animator.runtimeAnimatorController;
        if (!baseController || movingClip == null) return;

        AnimatorOverrideController overrideController = new AnimatorOverrideController(baseController);

        foreach (var clip in baseController.animationClips)
        {
            overrideController[clip.name] = movingClip;
        }

        animator.runtimeAnimatorController = overrideController;
        animator.Play(0, 0, 0f);
    }

    // =========================
    // COLLISION
    // =========================
    void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time - spawnTime < spawnGracePeriod)
            return;

        if (!projectileData.IsPiercing)
        {
            if (projectileType == ProjectileType.Enemy && other.CompareTag("Player"))
                Destroy(gameObject);

            if (projectileType == ProjectileType.Player && other.CompareTag("Enemy"))
                Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time - spawnTime < spawnGracePeriod)
            return;

        if (projectileType == ProjectileType.Player && !collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Destroy(gameObject);
                return;
            }

            HandleBounceOrDestroy();
        }

        if (projectileType == ProjectileType.Enemy)
        {
            HandleBounceOrDestroy();
        }
    }

    private void HandleBounceOrDestroy()
    {
        if (projectileData.MaxBounces > 0)
        {
            bouncesRemaining--;
            if (bouncesRemaining <= 0)
                Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =========================
    // FIXED MOVEMENT
    // =========================
    void FixedUpdate()
    {
        if (!projectileData || !rb) return;

        float accel = projectileData.Acceleration;
        if (Mathf.Approximately(accel, 0f)) return;

        Vector2 vel = rb.linearVelocity;
        float speed = vel.magnitude;
        if (speed <= 0f) return;

        Vector2 dir = vel / speed;
        float newSpeed = Mathf.Max(0f, speed + accel * Time.fixedDeltaTime);

        rb.linearVelocity = dir * newSpeed;

        if (projectileData.RotateTowardsMovementDirection)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
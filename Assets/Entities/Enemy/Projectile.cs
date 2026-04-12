using UnityEngine;

public class Projectile : MonoBehaviour
{
    private ProjectileData projectileData;
    private float projectileDamage;

    public ProjectileData ProjectileData { get => projectileData; set => projectileData = value; }
    public global::System.Single ProjectileDamage { get => projectileDamage; set => projectileDamage = value; }

    public void InitializeProjectile(Vector2 direction, Enemy enemy)
    {
        projectileData = enemy.EnemyData.ProjectileData;
        if (projectileData == null)
        {
            Debug.LogError("ProjectileData is not assigned in EnemyData.");
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
        if (sr != null && enemy.EnemyData != null)
        {
            sr.sprite = projectileData.ProjectileSprite;
        }

        float floor = enemy.Player != null ? enemy.Player.PlayerData.CurrentFloor : 0f;
        projectileDamage = projectileData.Damage + (projectileData.Damage * enemy.EnemyData.DamageScalar) * floor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
        
    }

    
}

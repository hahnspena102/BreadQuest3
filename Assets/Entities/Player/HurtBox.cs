using UnityEngine;

public class HurtBox : MonoBehaviour
{
    private Player player;

    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)

                player.TakeDamage(enemy.ContactDamage);
        }
        if (other.CompareTag("EnemyProjectile"))
        {
            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null)

                player.TakeDamage(projectile.ProjectileDamage);
        }
    }
}

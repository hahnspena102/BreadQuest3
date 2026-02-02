using System.Collections;
using UnityEngine;

public enum TargetMode
{
    Tag,
    Delta
}

[CreateAssetMenu(fileName = "ShootProjectile", menuName = "Scriptable Objects/Behaviors/ShootProjectile")]
public class ShootProjectile : EnemyBehavior
{
    [SerializeField] private TargetMode targetMode;

    [SerializeField] private string targetTag;
    [SerializeField] private Vector3 targetDelta;
    [SerializeField] private int projectileCount = 1;
    [SerializeField] private float spreadAngle = 15f;
    [SerializeField] private float projectileDelay = 0.1f;

    public override float PerformBehavior(Enemy enemy)
    {
        enemy.StartCoroutine(FireProjectiles(enemy));
        // Return total duration including delays
        return BehaviorDuration + enemy.EnemyData.ProjectileCooldown + projectileDelay * (projectileCount - 1);
    }

    private IEnumerator FireProjectiles(Enemy enemy)
    {
        GameObject projectilePrefab = enemy.ProjectilePrefab;
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Projectile prefab not assigned in Enemy");
            yield break;
        }

        Vector2 spawnPosition = enemy.transform.position;
        Vector2 targetPosition = Vector2.zero;

        switch (targetMode)
        {
            case TargetMode.Tag:
                GameObject target = GameObject.FindWithTag(targetTag);
                if (target == null)
                {
                    Debug.LogWarning($"No object with '{targetTag}' tag found");
                    yield break;
                }
                targetPosition = target.transform.position;
                break;

            case TargetMode.Delta:
                targetPosition = spawnPosition + (Vector2)targetDelta;
                break;
        }

        Vector2 baseDirection = (targetPosition - spawnPosition).normalized;

        for (int i = 0; i < projectileCount; i++)
        {
            float step = spreadAngle / projectileCount;
            float angleOffset = -spreadAngle / 2f + step * (i + 0.5f);

            Vector2 direction =
                Quaternion.Euler(0, 0, angleOffset) * baseDirection;

            GameObject projectileInstance =
                Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

            Projectile projectileComp = projectileInstance.GetComponent<Projectile>();
          
            projectileComp.InitializeProjectile(direction, enemy);

            if (i < projectileCount - 1)
                yield return new WaitForSeconds(projectileDelay);
        }
    }
}

using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Scriptable Objects/Behaviors/MeleeAttack")]
public class MeleeAttack : EnemyBehavior
{
    [SerializeField] private string targetTag;

    [Header("Melee Settings")]
    [SerializeField] private float range = 2f;
    [SerializeField] private float coneAngle = 60f;
    [SerializeField] private float damage = 10f;

    public override float PerformBehavior(Enemy enemy, float behaviorDuration)
    {
        enemy.StartCoroutine(MeleeRoutine(enemy));
        return behaviorDuration;
    }

    private IEnumerator MeleeRoutine(Enemy enemy)
    {
        Animator animator = enemy.GetComponent<Animator>();

        Vector2 targetPosition = FindTargetPosition(enemy);
        Vector2 directionToTarget = (targetPosition - (Vector2)enemy.transform.position).normalized;

        // Set animation
        if (animator != null)
        {
            animator.SetBool("attackB", directionToTarget.y > 0f);
            animator.SetBool("attackF", directionToTarget.y <= 0f);
        }

        // Wait for animation event
        while (!enemy.IsAttackReady)
            yield return null;

        enemy.IsAttackReady = false;

        // 🔥 Perform melee hit here
        PerformMeleeHit(enemy, directionToTarget);

        // Reset animation
        if (animator != null)
        {
            animator.SetBool("attackB", false);
            animator.SetBool("attackF", false);
        }
    }

    private void PerformMeleeHit(Enemy enemy, Vector2 forward)
    {
        Vector2 origin = enemy.transform.position;

        // Get all colliders in range
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag(targetTag)) continue;

            Vector2 directionToTarget = ((Vector2)hit.transform.position - origin).normalized;

            // Angle check (cone)
            float angle = Vector2.Angle(forward, directionToTarget);

            if (angle <= coneAngle * 0.5f)
            {
                // ✅ Target is inside cone → apply damage
                Player player = hit.GetComponentInParent<Player>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }
            }
        }
    }

    private Vector2 FindTargetPosition(Enemy enemy)
    {
        GameObject target = GameObject.FindWithTag(targetTag);
        if (target != null)
            return target.transform.position;

        return enemy.transform.position;
    }

    // Optional: visualize in editor
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, range);
    }
#endif
}
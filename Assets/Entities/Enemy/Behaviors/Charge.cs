using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Charge", menuName = "Scriptable Objects/Behaviors/Charge")]
public class Charge : EnemyBehavior
{
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float chargeSpeed = 12f;
    [SerializeField] private float chargeDuration = 1.0f;
    [SerializeField] private bool useFixedTargetPosition = true; // if true, sample target position once at start
    [SerializeField] private bool stopOnCollision = true;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float obstacleCheckRadius = 0.2f;
    [SerializeField] private float arrivalThreshold = 0.5f;
    [SerializeField] private float chargeWindup = 1f;
    [SerializeField] private float carryOverTime = 0.2f;
    [SerializeField] private float carryOverDamping = 5f;

    public override float PerformBehavior(Enemy enemy, float behaviorDuration)
    {
        // Use the provided behaviorDuration if > 0, otherwise fall back to configured chargeDuration
        float duration = behaviorDuration > 0f ? behaviorDuration : chargeDuration;
        enemy.StartCoroutine(ChargeRoutine(enemy, duration));
        return duration;
    }

    private IEnumerator ChargeRoutine(Enemy enemy, float duration)
    {
        Animator animator = enemy.GetComponent<Animator>();

        GameObject target = GameObject.FindWithTag(targetTag);
        if (target == null)
        {
            Debug.LogWarning("Charge: target not found with tag '" + targetTag + "'");
            yield break;
        }



        animator.SetBool("attackF", true);
        yield return new WaitForSeconds(chargeWindup); 

        SoundManager.instance.PlaySoundFXClip(enemy.EnemyData.GetAttackSound(), enemy.transform);
        float timer = 0f;
  

        Vector2 targetPos = target.transform.position;
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();

        while (timer < duration)
        {
            animator.SetFloat("speed", chargeSpeed);
            enemy.transform.localScale = new Vector3(Mathf.Sign(targetPos.x - enemy.transform.position.x), 1f, 1f) * enemy.ScaleFactor;
            Vector2 enemyPos = enemy.transform.position;
            if (!useFixedTargetPosition)
                targetPos = target.transform.position;

            Vector2 toTarget = targetPos - enemyPos;
            float dist = toTarget.magnitude;
            if (dist <= arrivalThreshold)
                break;

            Vector2 dir = toTarget.normalized;

            // obstacle check in front of the enemy
            if (stopOnCollision && Physics2D.CircleCast(enemyPos, obstacleCheckRadius, dir, obstacleCheckRadius, obstacleLayer))
            {
                break;
            }

            if (rb != null)
            {
                rb.linearVelocity = dir * chargeSpeed;
            }
            else
            {
                enemy.transform.position = enemy.transform.position + (Vector3)(dir * chargeSpeed * Time.deltaTime);
            }

            timer += Time.deltaTime;
            yield return null;
        }
        animator.SetBool("attackF", false);

        // stop movement
        Rigidbody2D finalRb = enemy.GetComponent<Rigidbody2D>();

        if (finalRb != null)
        {
            Vector2 velocity = finalRb.linearVelocity;
            float t = 0f;

            while (t < carryOverTime)
            {
                // Gradually reduce velocity
                velocity = Vector2.Lerp(velocity, Vector2.zero, carryOverDamping * Time.deltaTime);
                finalRb.linearVelocity = velocity;

                t += Time.deltaTime;
                yield return null;
            }

            finalRb.linearVelocity = Vector2.zero;
        }
        yield break;
    }
}

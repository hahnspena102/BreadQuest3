using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "PatrolAroundTarget", menuName = "Scriptable Objects/Behaviors/PatrolAroundTarget")]
public class PatrolAroundTarget : EnemyBehavior
{
    [SerializeField] private string targetTag;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 4f;
    [SerializeField] private float updateInterval = 1.5f;
    [SerializeField] private bool randomizeDirection = true;

    public override float PerformBehavior(Enemy enemy, float behaviorDuration)
    {
        Pathfinder pathfinder = enemy.GetComponent<Pathfinder>();
        if (pathfinder == null)
        {
            Debug.LogWarning("Pathfinder component not found on " + enemy.name);
            return 0f;
        }

        enemy.StartCoroutine(PatrolRoutine(enemy, pathfinder, behaviorDuration));
        return behaviorDuration;
    }

    private IEnumerator PatrolRoutine(Enemy enemy, Pathfinder pathfinder, float duration)
    {
        float timer = 0f;

        GameObject target = GameObject.FindWithTag(targetTag);
        if (target == null)
        {
            Debug.LogWarning($"No object with '{targetTag}' tag found");
            yield break;
        }

        int direction = randomizeDirection ? (Random.value > 0.5f ? 1 : -1) : 1;
        float currentAngle = Random.Range(0f, 360f);

        while (timer < duration)
        {
            Vector2 targetPos = target.transform.position;

            // Move angle forward to "circle"
            currentAngle += direction * Random.Range(30f, 90f);

            float rad = currentAngle * Mathf.Deg2Rad;

            Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * patrolRadius;
            Vector2 patrolPoint = targetPos + offset;

            pathfinder.MoveToTarget(patrolPoint);

            yield return new WaitForSeconds(updateInterval);
            timer += updateInterval;
        }

        pathfinder.Stop();
    }
}
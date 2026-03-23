using UnityEngine;
using System.Collections;

public enum MoveTargetMode
{
    Delta,
    Tag
}

[CreateAssetMenu(fileName = "Move", menuName = "Scriptable Objects/Behaviors/Move")]
public class Move : EnemyBehavior
{
    [SerializeField] private MoveTargetMode targetMode;

    // ───── Delta ─────
    [SerializeField] private Vector2 delta;

    // ───── Tag ─────
    [SerializeField] private string targetTag;
    [SerializeField] private bool towardsTarget = true;
    [SerializeField] private float fleeDistance = 5f;

    [SerializeField] private bool continuousPursuit = true; // new: follow player continuously
    [SerializeField] private float updateInterval = 0.1f;   // how often to recalc target

    public override float PerformBehavior(Enemy enemy, float behaviorDuration)
    {
        Pathfinder pathfinder = enemy.GetComponent<Pathfinder>();
        if (pathfinder == null)
        {
            Debug.LogWarning("Pathfinder component not found on " + enemy.name);
            return 0f;
        }

        // Start a coroutine to handle movement
        enemy.StartCoroutine(MoveRoutine(enemy, pathfinder, behaviorDuration));

        // Return how long this behavior should last
        return behaviorDuration;
    }

    private IEnumerator MoveRoutine(Enemy enemy, Pathfinder pathfinder, float behaviorDuration = 5f)
    {
        float timer = 0f;

        while (timer < behaviorDuration)
        {
            Vector2 enemyPos = enemy.transform.position;

            switch (targetMode)
            {
                case MoveTargetMode.Delta:
                    pathfinder.MoveToTarget(enemyPos + delta);
                    break;

                case MoveTargetMode.Tag:
                    GameObject target = GameObject.FindWithTag(targetTag);

                    if (target == null)
                    {
                        Debug.LogWarning($"No object with '{targetTag}' tag found");
                        yield break;
                    }

                    Vector2 targetPos = target.transform.position;

                    if (towardsTarget)
                    {
                        if (continuousPursuit)
                        {
                            pathfinder.MoveToTarget(targetPos); // update target every frame
                        }
                        else if (timer == 0f)
                        {
                            pathfinder.MoveToTarget(targetPos); // only once at start
                        }
                    }
                    else
                    {
                        Vector2 awayDir = (enemyPos - targetPos).normalized;
                        Vector2 fleeTargetPos = enemyPos + awayDir * fleeDistance;
                        pathfinder.MoveToTarget(fleeTargetPos);
                    }
                    break;
            }

            timer += updateInterval;
            yield return new WaitForSeconds(updateInterval);
        }

        // Stop moving at the end of behavior
        pathfinder.Stop();
    }
}

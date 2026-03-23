using UnityEngine;
using System.Collections;


[CreateAssetMenu(fileName = "WeepingAngel", menuName = "Scriptable Objects/Behaviors/WeepingAngel")]
public class WeepingAngel : EnemyBehavior
{

    // ───── Tag ─────
    [SerializeField] private string targetTag = "Player";
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


    IEnumerator MoveRoutine(Enemy enemy, Pathfinder pathfinder, float behaviorDuration = 5f)
    {
        float timer = 0f;

        GameObject target = null;


        target = GameObject.FindWithTag(targetTag);
        if (target == null)
        {
            Debug.LogWarning($"No object with '{targetTag}' tag found");
            yield break;
        }
        

        while (timer < behaviorDuration)
        {
            Vector2 enemyPos = enemy.transform.position;

            if (target != null)
            {
                bool isLookedAt = IsPlayerLookingAtEnemy(enemy.transform);

                if (isLookedAt)
                {
                    pathfinder.Stop();
                    Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
                    if (rb != null) rb.linearVelocity = Vector2.zero; 
                }
                else
                {
                    Vector2 targetPos = target.transform.position;
                    pathfinder.MoveToTarget(targetPos); 
                    
                }
            }
            

            timer += updateInterval;
            yield return new WaitForSeconds(updateInterval);
        }

        pathfinder.Stop();
    }

    bool IsPlayerLookingAtEnemy(Transform enemy)
    {
        Player player = GameObject.FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogWarning("Player not found in scene");
            return false;
        }

        if (player.DirectionFacing == "Up" && enemy.position.y > player.transform.position.y) return true;
        if (player.DirectionFacing == "Down" && enemy.position.y < player.transform.position.y) return true;
        if (player.DirectionFacing == "Left" && enemy.position.x < player.transform.position.x) return true;
        if (player.DirectionFacing == "Right" && enemy.position.x > player.transform.position.x) return true;

        return false;
    }
}


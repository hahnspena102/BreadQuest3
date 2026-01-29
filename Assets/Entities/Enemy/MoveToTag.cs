using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Scriptable Objects/Move")]
public class Move : EnemyBehavior
{
    [SerializeField]private string targetTag;
    [SerializeField]private bool towardsTarget = true;
    [SerializeField] private float fleeDistance = 5f;
    public override void PerformBehavior(GameObject enemy)
    {
        Pathfinder pathfinder = enemy.GetComponent<Pathfinder>();

        if (pathfinder == null)
        {
            Debug.LogWarning("Pathfinder component not found on " + enemy.name);
            return;
        }

        GameObject target = GameObject.FindWithTag(targetTag);

        Vector2 enemyPos = enemy.transform.position;
        Vector2 targetPos = target.transform.position;


        if (target != null)
        {
            if (towardsTarget)
            {
                pathfinder.MoveToTarget(targetPos);
                Debug.Log("Moving towards targetPos: " + targetPos);
            } else
            {
                Vector2 awayDir = (enemyPos - targetPos).normalized;

                // Pick a point away from the target
                Vector2 fleeTargetPos = enemyPos + awayDir * fleeDistance;

                pathfinder.MoveToTarget(fleeTargetPos);
            }
         
        }
        else
        {
            Debug.LogWarning("No object with 'Player' tag found in the scene");
        }

        
    }

    
}

using UnityEngine;

[CreateAssetMenu(fileName = "MoveDelta", menuName = "Scriptable Objects/MoveDelta")]
public class MoveDelta : EnemyBehavior
{
    [SerializeField]private float deltaX, deltaY;    
    
    public override void PerformBehavior(GameObject enemy)
    {
        Pathfinder pathfinder = enemy.GetComponent<Pathfinder>();

        if (pathfinder == null)
        {
            Debug.LogWarning("Pathfinder component not found on " + enemy.name);
            return;
        }

        Vector2 enemyPos = enemy.transform.position;

        pathfinder.MoveToTarget(enemyPos + new Vector2(deltaX, deltaY));

        
    }

    
}

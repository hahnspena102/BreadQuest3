using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(SpriteRenderer))]
public class Pathfinder : MonoBehaviour
{
    [ReadOnly][SerializeField] private EnemyData enemyData;
    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;

    private bool hasTarget = false;
    private bool hasLoggedAgentNotReady = false;

    public EnemyData EnemyData { get => enemyData; set => enemyData = value; }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(StartPathfinding());
    }

    IEnumerator StartPathfinding()
    {
        while (enemyData == null)
            yield return null;

        // 2D setup
        agent.updateRotation = false;  // prevent 3D rotation
        agent.updateUpAxis = false;    // keep Z = 0
        agent.speed = enemyData.Speed;

        // Make sure enemy is visible
        spriteRenderer.enabled = true;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    public void MoveToTarget(Vector2 destPos)
    {
        if (!IsAgentReady())
        {
            hasTarget = false;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(destPos);
        hasTarget = true;
    }

    public void Stop()
    {
        hasTarget = false;

        if (!IsAgentReady())
        {
            return;
        }

        agent.isStopped = true;
    }

    void Update()
    {
        if (!IsAgentReady())
        {
            hasTarget = false;
            return;
        }

        if (agent.path.status == NavMeshPathStatus.PathInvalid)
        {
            hasTarget = false;
//            agent.isStopped = true;
            return;
        }
        
        if (hasTarget && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            hasTarget = false;
            agent.isStopped = true;
        }
    }

    private bool IsAgentReady()
    {
        bool ready = agent != null
            && agent.enabled
            && gameObject.activeInHierarchy
            && agent.isActiveAndEnabled
            && agent.isOnNavMesh;

        if (!ready)
        {
            if (!hasLoggedAgentNotReady)
            {
                Debug.LogWarning("Pathfinder skipped movement: NavMeshAgent is disabled or not on a NavMesh for " + gameObject.name);
                hasLoggedAgentNotReady = true;
            }

            return false;
        }

        hasLoggedAgentNotReady = false;
        return true;
    }
}

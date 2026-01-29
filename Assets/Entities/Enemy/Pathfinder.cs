using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stoppingDistance = 0.1f;

    private Rigidbody2D rb;

    private Vector2 targetPos;
    private bool hasTarget = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!hasTarget) return;

        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPos - currentPos).normalized;


        float distance = Vector2.Distance(currentPos, targetPos);

        if (distance > stoppingDistance)
        {
            if (rb != null)
            {
                rb.linearVelocity = direction * moveSpeed;
            }
            else
            {
                transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }
    }

    // Call this once to start moving
    public void MoveToTarget(Vector2 destPos)
    {
        targetPos = destPos;
        hasTarget = true;
    }

    public void Stop()
    {
        hasTarget = false;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
}

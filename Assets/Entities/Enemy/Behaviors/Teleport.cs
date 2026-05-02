using UnityEngine;

[CreateAssetMenu(fileName = "Teleport", menuName = "Scriptable Objects/Behaviors/Teleport")]
public class Teleport : EnemyBehavior
{
    [SerializeField] private float minTeleportDistance = 3f;
    [SerializeField] private float maxTeleportDistance = 10f;
    [SerializeField] private int maxAttempts = 20;
    [SerializeField] private LayerMask obstacleLayer;

    private WorldManager worldManager;

    public override float PerformBehavior(Enemy enemy, float behaviorDuration)
    {
        if (worldManager == null)
            worldManager = FindFirstObjectByType<WorldManager>();

        if (worldManager == null)
        {
            Debug.LogWarning("WorldManager not found in scene.");
            return 0f;
        }

        if (!TryFindRandomValidTile(enemy, out Vector3 newPosition))
        {
            // Fallback: stay in place instead of risking a bad teleport coordinate.
            newPosition = enemy.transform.position;
            Debug.Log("Teleport fallback for " + enemy.name + ": no valid tile found, staying in place at " + newPosition);
        }

        // Extra safety guard against accidental zero-vector teleports.
        if (newPosition == Vector3.zero)
        {
            newPosition = enemy.transform.position;
            Debug.Log("Teleport fallback for " + enemy.name + ": prevented teleport to (0,0,0), staying in place at " + newPosition);
        }

        enemy.transform.position = newPosition;

        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        return behaviorDuration;
    }

    private bool TryFindRandomValidTile(Enemy enemy, out Vector3 result)
    {
        result = enemy.transform.position;

        RectInt roomArea = default;
        bool hasRoom = enemy.AssignedRoom != null;
        if (hasRoom)
        {
            roomArea = enemy.AssignedRoom.area;
        }

        int centerX = Mathf.FloorToInt(enemy.transform.position.x);
        int centerY = Mathf.FloorToInt(enemy.transform.position.y);

        int minD = Mathf.CeilToInt(minTeleportDistance);
        int maxD = Mathf.CeilToInt(maxTeleportDistance);

        // Primary pass: choose a position around current enemy location within min/max distance.
        for (int i = 0; i < maxAttempts; i++)
        {
            int dx = Random.Range(-maxD, maxD + 1);
            int dy = Random.Range(-maxD, maxD + 1);
            int distSq = dx * dx + dy * dy;

            if (distSq < minD * minD || distSq > maxD * maxD)
                continue;

            int x = centerX + dx;
            int y = centerY + dy;

            if (hasRoom)
            {
                if (!roomArea.Contains(new Vector2Int(x, y)))
                    continue;
            }
            else
            {
                ClampToWorldBounds(ref x, ref y);
            }

            Vector3 pos = new Vector3(x + 0.5f, y + 0.5f, 0f);
            if (IsWalkableTile(pos))
            {
                result = pos;
                return true;
            }
        }

        // Secondary pass: if the enemy has a room, allow any tile in that room.
        if (hasRoom)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                int x = Random.Range(roomArea.xMin, roomArea.xMax);
                int y = Random.Range(roomArea.yMin, roomArea.yMax);
                Vector3 pos = new Vector3(x + 0.5f, y + 0.5f, 0f);
                if (IsWalkableTile(pos))
                {
                    result = pos;
                    return true;
                }
            }

            Debug.Log("Teleport fallback for " + enemy.name + ": no valid tile in assigned room, staying in place.");
            return false;
        }

        // Final fallback: no room available, pick random world tiles.
        for (int i = 0; i < maxAttempts; i++)
        {
            int x = Random.Range(0, 108);
            int y = Random.Range(0, 72);
            Vector3 pos = new Vector3(x + 0.5f, y + 0.5f, 0f);
            if (IsWalkableTile(pos))
            {
                result = pos;
                return true;
            }
        }

        Debug.Log("Teleport fallback for " + enemy.name + ": no room assigned and no global tile found, staying in place.");
        return false;
    }

    private void ClampToWorldBounds(ref int x, ref int y)
    {
        x = Mathf.Clamp(x, 0, 107);
        y = Mathf.Clamp(y, 0, 71);
    }

    private bool IsWalkableTile(Vector3 position)
    {
        // Use a slightly larger radius to catch colliders that cover most of the tile
        float radius = 0.32f;
        return !Physics2D.OverlapCircle(position, radius, obstacleLayer);
    }
}
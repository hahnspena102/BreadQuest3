using UnityEngine;

[CreateAssetMenu(fileName = "SetVelocity", menuName = "Scriptable Objects/Behaviors/SetVelocity")]
public class SetVelocity : EnemyBehavior
{
    [SerializeField] private Vector2 direction;
    [SerializeField] private float angleRandomness = 0f;

    public override float PerformBehavior(Enemy enemy, float behaviorDuration)
    {
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogWarning("Rigidbody2D not found on " + enemy.name);
            return 0f;
        }

        Vector2 velocity = direction.normalized * enemy.EnemyData.Speed;
        if (angleRandomness > 0)
        {
            float randomAngle = Random.Range(-angleRandomness, angleRandomness);
            velocity = Quaternion.Euler(0, 0, randomAngle) * velocity;
        }


        rb.linearVelocity = velocity;

        return behaviorDuration;
    }
}
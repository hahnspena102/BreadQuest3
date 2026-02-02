using UnityEngine;

public abstract class EnemyBehavior : ScriptableObject
{
    [SerializeField] private float behaviorDuration = 1f;
    public float BehaviorDuration => behaviorDuration;
    public abstract float PerformBehavior(Enemy enemy);
}

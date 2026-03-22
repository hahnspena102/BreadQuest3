using UnityEngine;

public abstract class EnemyBehavior : ScriptableObject
{
    [SerializeField][MinMax(0, 20)] private MinMaxFloat behaviorDuration;
    public float BehaviorDuration => behaviorDuration.RandomValue;
    public abstract float PerformBehavior(Enemy enemy);
}

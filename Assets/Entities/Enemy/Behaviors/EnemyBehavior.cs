using UnityEngine;

[System.Serializable]
public class EnemyBehaviorEntry
{
    public EnemyBehavior Behavior;
    [SerializeField][MinMax(0, 20)] private MinMaxFloat behaviorDuration;
    public int TimesToRepeat = 1;
    public float BehaviorDuration => behaviorDuration.RandomValue;


}

public abstract class EnemyBehavior : ScriptableObject
{
    public abstract float PerformBehavior(Enemy enemy, float behaviorDuration);
}

using UnityEngine;

public abstract class EnemyBehavior : ScriptableObject
{
    public abstract void PerformBehavior(GameObject gameObject);
}

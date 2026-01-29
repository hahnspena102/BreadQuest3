using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField]private string enemyName;
    [SerializeField]private string description;
    [SerializeField]private List<EnemyBehavior> enemyBehaviors;

    public List<EnemyBehavior> EnemyBehaviors { get => enemyBehaviors; set => enemyBehaviors = value; }
}

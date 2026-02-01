using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : EntityData
{
    [SerializeField]private string enemyName;
    [SerializeField]private Sprite enemySprite;
    [SerializeField]private string description;
    [SerializeField]private Flavor flavor;
    [SerializeField]private List<EnemyBehavior> enemyBehaviors;
    [Header("Animations")]
    [SerializeField]private AnimationClip idleAnimationF;
    [SerializeField]private AnimationClip idleAnimationB;

    public List<EnemyBehavior> EnemyBehaviors { get => enemyBehaviors; set => enemyBehaviors = value; }
    public Sprite EnemySprite { get => enemySprite; set => enemySprite = value; }
    public AnimationClip IdleAnimationF { get => idleAnimationF; set => idleAnimationF = value; }
    public AnimationClip IdleAnimationB { get => idleAnimationB; set => idleAnimationB = value; }
}
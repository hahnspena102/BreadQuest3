using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : EntityData
{
    [SerializeField]private string enemyName;
    [SerializeField]private Sprite enemySprite;
    [SerializeField]private string description;
    [SerializeField]private Flavor flavor;
    [SerializeField]private int contactDamage;
    [SerializeField] private ProjectileData projectileData;
    [SerializeField]private float projectileCooldown;
    [SerializeField]private List<EnemyBehavior> enemyBehaviors;
    [SerializeField]private bool performBehaviorsInOrder = false;
    [Header("Animations")]
    [SerializeField]private AnimationClip idleAnimationF;
    [SerializeField]private AnimationClip idleAnimationB;

    public List<EnemyBehavior> EnemyBehaviors { get => enemyBehaviors; set => enemyBehaviors = value; }
    public Sprite EnemySprite { get => enemySprite; set => enemySprite = value; }
    public AnimationClip IdleAnimationF { get => idleAnimationF; set => idleAnimationF = value; }
    public AnimationClip IdleAnimationB { get => idleAnimationB; set => idleAnimationB = value; }
    public global::System.Single ProjectileCooldown { get => projectileCooldown; set => projectileCooldown = value; }
    public global::System.Boolean PerformBehaviorsInOrder { get => performBehaviorsInOrder; set => performBehaviorsInOrder = value; }
    public ProjectileData ProjectileData { get => projectileData; set => projectileData = value; }
    public global::System.Int32 ContactDamage { get => contactDamage; set => contactDamage = value; }
}
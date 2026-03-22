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
    [SerializeField]private AnimationClip moveAnimationF;
    [SerializeField]private AnimationClip moveAnimationB;
    [SerializeField]private AnimationClip attackAnimationF;
    [SerializeField]private AnimationClip attackAnimationB;
    [SerializeField]private Vector2 shadowOffset;
    [SerializeField]private Vector2 shadowScale = Vector2.one;

    public Vector2 ShadowOffset { get => shadowOffset; set => shadowOffset = value; }
    public Vector2 ShadowScale { get => shadowScale; set => shadowScale = value; }
    public List<EnemyBehavior> EnemyBehaviors { get => enemyBehaviors; set => enemyBehaviors = value; }
    public Sprite EnemySprite { get => enemySprite; set => enemySprite = value; }
    public AnimationClip IdleAnimationF { get => idleAnimationF; set => idleAnimationF = value; }
    public AnimationClip IdleAnimationB { get => idleAnimationB; set => idleAnimationB = value; }
    public global::System.Single ProjectileCooldown { get => projectileCooldown; set => projectileCooldown = value; }
    public global::System.Boolean PerformBehaviorsInOrder { get => performBehaviorsInOrder; set => performBehaviorsInOrder = value; }
    public ProjectileData ProjectileData { get => projectileData; set => projectileData = value; }
    public global::System.Int32 ContactDamage { get => contactDamage; set => contactDamage = value; }
    public global::System.String EnemyName { get => enemyName; set => enemyName = value; }
    public AnimationClip MoveAnimationF { get => moveAnimationF; set => moveAnimationF = value; }
    public AnimationClip MoveAnimationB { get => moveAnimationB; set => moveAnimationB = value; }
    public AnimationClip AttackAnimationF { get => attackAnimationF; set => attackAnimationF = value; }
    public AnimationClip AttackAnimationB { get => attackAnimationB; set => attackAnimationB = value; }
}
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
    [SerializeField]private float experienceDropped = 100f;
    [SerializeField] private ProjectileData projectileData;
    [SerializeField] private Vector2 projectileOffset = Vector2.zero;
    [SerializeField]private float projectileCooldown;
    [SerializeField]private List<EnemyBehaviorEntry> onSpawnBehaviors;
    [SerializeField]private List<EnemyBehaviorEntry> behaviors;
    [SerializeField]private bool disableAgent = false;
    [SerializeField]private bool isBouncy = false;

    [SerializeField]private bool performBehaviorsInOrder = false;
    [Header("Animations")]
    [SerializeField]private RuntimeAnimatorController animatorOverride;
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
    public List<EnemyBehaviorEntry> OnSpawnBehaviors { get => onSpawnBehaviors; set => onSpawnBehaviors = value; }
    public List<EnemyBehaviorEntry> Behaviors { get => behaviors; set => behaviors = value; }
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
    public Vector2 ProjectileOffset { get => projectileOffset; set => projectileOffset = value; }
    public global::System.Boolean IsBouncy { get => isBouncy; set => isBouncy = value; }
    public global::System.Boolean DisableAgent { get => disableAgent; set => disableAgent = value; }
    public RuntimeAnimatorController AnimatorOverride { get => animatorOverride; set => animatorOverride = value; }
    public global::System.Single ExperienceDropped { get => experienceDropped; set => experienceDropped = value; }
    public Flavor Flavor { get => flavor; set => flavor = value; }
}
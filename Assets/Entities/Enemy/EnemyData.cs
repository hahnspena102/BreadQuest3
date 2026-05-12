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
    [SerializeField]private int baseDamage;
    [SerializeField]private float experienceDropped = 100f;
    [SerializeField] private ProjectileData projectileData;
    [SerializeField] private Vector2 projectileOffset = Vector2.zero;
    [SerializeField]private float projectileCooldown;
    [SerializeField]private List<EnemyBehaviorEntry> onSpawnBehaviors;
    [SerializeField]private List<EnemyBehaviorEntry> behaviors;
    [SerializeField]private bool disableAgent = false;
    [SerializeField]private bool isBouncy = false;
    [SerializeField]private bool isDynamic = false;
    [SerializeField]private float healthScalar = 0.1f;
    [SerializeField]private float damageScalar = 0.1f;
    [SerializeField]private bool canBeMiniBoss = true;
    [SerializeField]private bool ignoreEnemyCollision = false;
    [SerializeField]private float defense = 0f;
    public float HealthScalar { get => healthScalar; set => healthScalar = value; }
    public float DamageScalar { get => damageScalar; set => damageScalar = value; }

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

    [Header("Sounds")]
    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] hurtSounds;
    [SerializeField] private AudioClip[] deathSounds;
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
    public global::System.Boolean IgnoreEnemyCollision { get => ignoreEnemyCollision; set => ignoreEnemyCollision = value; }
    public global::System.Int32 BaseDamage { get => baseDamage; set => baseDamage = value; }
    public global::System.Boolean IsDynamic { get => isDynamic; set => isDynamic = value; }
    public global::System.Boolean CanBeMiniBoss { get => canBeMiniBoss; set => canBeMiniBoss = value; }
    public global::System.Single Defense { get => defense; set => defense = value; }

    public AudioClip GetAttackSound()
    {
        if (attackSounds == null || attackSounds.Length == 0) return null;
        return attackSounds[Random.Range(0, attackSounds.Length)];
    }

    public AudioClip GetHurtSound()
    {
        if (hurtSounds == null || hurtSounds.Length == 0) return null;
        return hurtSounds[Random.Range(0, hurtSounds.Length)];
    }

    public AudioClip GetDeathSound()
    {
        if (deathSounds == null || deathSounds.Length == 0) return null;
        return deathSounds[Random.Range(0, deathSounds.Length)];
    }

    public bool MatchesFlavors(Flavor[] flavors)
    {
        if (flavors == null || flavors.Length == 0) return true;
        foreach (var f in flavors)
        {
            if (Flavor == f) return true;
        }
        return false;
    }
}
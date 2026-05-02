using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Scriptable Objects/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    [SerializeField] private Sprite projectileSprite;
    [SerializeField]private AnimationClip movingAnimation;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool rotateTowardsMovementDirection;
    [SerializeField] private int maxBounces;
    [SerializeField]private bool isTrigger;

    public global::System.Single Speed { get => speed; set => speed = value; }
    public global::System.Single Lifetime { get => lifetime; set => lifetime = value; }
    public Sprite ProjectileSprite { get => projectileSprite; set => projectileSprite = value; }
    public global::System.Single RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
    public global::System.Boolean RotateTowardsMovementDirection { get => rotateTowardsMovementDirection; set => rotateTowardsMovementDirection = value; }
    public global::System.Int32 MaxBounces { get => maxBounces; set => maxBounces = value; }
    public AnimationClip MovingAnimation { get => movingAnimation; set => movingAnimation = value; }
    public global::System.Boolean IsTrigger { get => isTrigger; set => isTrigger = value; }
}

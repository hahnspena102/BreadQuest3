using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Scriptable Objects/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float lifetime;
    [SerializeField] private Sprite projectileSprite;
    [SerializeField] private float rotationSpeed;

    public global::System.Single Speed { get => speed; set => speed = value; }
    public global::System.Single Damage { get => damage; set => damage = value; }
    public global::System.Single Lifetime { get => lifetime; set => lifetime = value; }
    public Sprite ProjectileSprite { get => projectileSprite; set => projectileSprite = value; }
    public global::System.Single RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
}

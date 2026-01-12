using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
public class Weapon : Item
{
    [SerializeField] private float damage;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float range;

    public global::System.Single Damage { get => damage; set => damage = value; }
    public global::System.Single AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
    public global::System.Single Range { get => range; set => range = value; }
}

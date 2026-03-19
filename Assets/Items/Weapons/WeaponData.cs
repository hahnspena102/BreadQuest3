using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/Weapon Data")]
public class WeaponData : ItemData
{
    [SerializeField] private float damage;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float range;
    [SerializeField] private Flavor flavor;

    public global::System.Single Damage { get => damage; set => damage = value; }
    public global::System.Single AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
    public global::System.Single Range { get => range; set => range = value; }
}

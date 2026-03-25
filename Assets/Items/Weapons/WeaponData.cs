using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/Weapon Data")]
public class WeaponData : ItemData
{
    [SerializeField] private float damage;
    [SerializeField] private Flavor flavor;

    public global::System.Single Damage { get => damage; set => damage = value; }
    public Flavor Flavor { get => flavor; set => flavor = value; }
}

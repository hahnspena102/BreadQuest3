using UnityEngine;

[System.Serializable]
public class Weapon : Item
{
    [SerializeField]private int level = 1;
    public WeaponData WeaponData => ItemData as WeaponData;

    public global::System.Int32 Level { get => level; set => level = value; }
}
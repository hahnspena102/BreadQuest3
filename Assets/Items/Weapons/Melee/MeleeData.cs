using UnityEngine;

[CreateAssetMenu(fileName = "MeleeData", menuName = "Scriptable Objects/MeleeData")]
public class MeleeData : WeaponData
{
    [SerializeField] private float meleeScale = 1f;
    [SerializeField] private float meleeSpeed = 1f;
    public float MeleeSpeed { get => meleeSpeed; set => meleeSpeed = value; }
    public float MeleeScale { get => meleeScale; set => meleeScale = value; }
}

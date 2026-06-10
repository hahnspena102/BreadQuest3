using UnityEngine;

[CreateAssetMenu(fileName = "MeleeData", menuName = "Scriptable Objects/Weapons/MeleeData")]
public class MeleeData : WeaponData
{
    [SerializeField] private float meleeScale = 1f;
    [SerializeField] private float meleeSpeed = 1f;
    public float MeleeSpeed { get => meleeSpeed; set => meleeSpeed = value; }
    public float MeleeScale { get => meleeScale; set => meleeScale = value; }

    public override string GetFullDescription()
    {
        return $"{base.GetFullDescription()}\nMelee - {GetSizeDescription()} - {GetSpeedDescription()}";
    }

    public string GetSizeDescription()
    {
        if (MeleeScale < 0.75f)
        {
            return "small";
        }
        else if (MeleeScale < 1.25f)
        {
            return "medium";
        }
        else
        {
            return "large";
        }
    }

    public string GetSpeedDescription()
    {
        if (MeleeSpeed < 0.75f)
        {
            return "slow";
        }
        else if (MeleeSpeed < 1.25f)
        {
            return "moderate";
        }
        else
        {
            return "fast";
        }
    }
}

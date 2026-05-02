using UnityEngine;

[CreateAssetMenu(fileName = "MagicData", menuName = "Scriptable Objects/Weapons/MagicData")]
public class MagicData : WeaponData
{
    public ProjectileData ProjectileData { get => projectileData; set => projectileData = value; }
    public global::System.Single GlucoseCost { get => glucoseCost; set => glucoseCost = value; }
    public global::System.Int32 ProjectileCount { get => projectileCount; set => projectileCount = value; }
    public global::System.Single SpreadAngle { get => spreadAngle; set => spreadAngle = value; }
    public global::System.Single ProjectileDelay { get => projectileDelay; set => projectileDelay = value; }

    [SerializeField]private ProjectileData projectileData; 
    [SerializeField]private float glucoseCost;
    [SerializeField] private int projectileCount = 1;
    [SerializeField] private float spreadAngle = 15f;
    [SerializeField] private float projectileDelay = 0.1f;

    [Header("Sounds")]
    [SerializeField] private AudioClip[] castSounds;
    public AudioClip GetCastSound()
    {
        if (castSounds == null || castSounds.Length == 0)
            return null;
        return castSounds[Random.Range(0, castSounds.Length)];
    }
}

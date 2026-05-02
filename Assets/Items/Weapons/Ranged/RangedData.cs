using UnityEngine;

[CreateAssetMenu(fileName = "RangedData", menuName = "Scriptable Objects/Weapons/RangedData")]
public class RangedData : WeaponData
{
    public ProjectileData ProjectileData { get => projectileData; set => projectileData = value; }
    public global::System.Int32 ProjectileCount { get => projectileCount; set => projectileCount = value; }
    public global::System.Single SpreadAngle { get => spreadAngle; set => spreadAngle = value; }
    public global::System.Single ProjectileDelay { get => projectileDelay; set => projectileDelay = value; }
    public global::System.Single ChargeTime { get => chargeTime; set => chargeTime = value; }
    public AnimationClip ChargeAnimationClip { get => chargeAnimationClip; set => chargeAnimationClip = value; }
    public AnimationClip ReleaseAnimationClip { get => releaseAnimationClip; set => releaseAnimationClip = value; }

    [SerializeField]private ProjectileData projectileData; 
    [SerializeField] private int projectileCount = 1;
    [SerializeField] private float spreadAngle = 15f;
    [SerializeField] private float projectileDelay = 0.1f;
    [SerializeField]private float chargeTime = 0f;
    [SerializeField]private AnimationClip chargeAnimationClip;
    [SerializeField]private AnimationClip releaseAnimationClip;
}

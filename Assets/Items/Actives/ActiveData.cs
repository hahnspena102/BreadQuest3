using UnityEngine;

[CreateAssetMenu(fileName = "ActiveData", menuName = "Scriptable Objects/Active Data")]
public class ActiveData : ItemData
{
    [SerializeField] private float cooldownDuration;

    public global::System.Single CooldownDuration { get => cooldownDuration; set => cooldownDuration = value; }
}

using UnityEngine;

public class Active : Item
{
    public ActiveData ActiveData => ItemData as ActiveData;
    public float Cooldown { get; set; }
    public float CooldownDuration => ActiveData != null ? ActiveData.CooldownDuration : 0f;
}
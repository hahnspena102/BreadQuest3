using UnityEngine;

public enum PotionEffect
{
    Heal,
    Glucose,
}

[System.Serializable]
public class PotionEntry 
{
    public PotionEffect effect;
    public float magnitude;
    public float duration;
}

[CreateAssetMenu(fileName = "PotionData", menuName = "Scriptable Objects/Potion Data")]
public class PotionData : ActiveData
{
    public PotionEntry[] potionEffects;

    public override string GetFullDescription()
    {
        string effectsDescription = "";
        foreach (var entry in potionEffects)
        {
            if (entry.duration <= 0)
            {
                effectsDescription += $"\n{entry.effect} - {entry.magnitude}";
            }
            else
            {
                effectsDescription += $"\n{entry.effect} - {entry.magnitude} for {entry.duration} sec";
            }
            
        }
        return $"{base.GetFullDescription()}\n{effectsDescription}";
    }

}

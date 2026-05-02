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

}

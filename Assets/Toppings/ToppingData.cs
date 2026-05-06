using UnityEngine;

[CreateAssetMenu(fileName = "ToppingData", menuName = "Scriptable Objects/ToppingData", order = 1)]
public class ToppingData : ScriptableObject
{
    [SerializeField] private string toppingName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private float healthRegenBonus;
    [SerializeField] private float glucoseRegenBonus;
    public string ToppingName => toppingName;
    public string Description => description;
    public Sprite Icon => icon;

    public global::System.Single HealthRegenBonus { get => healthRegenBonus; set => healthRegenBonus = value; }
    public global::System.Single GlucoseRegenBonus { get => glucoseRegenBonus; set => glucoseRegenBonus = value; }
}
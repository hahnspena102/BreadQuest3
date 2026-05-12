using UnityEngine;

[CreateAssetMenu(fileName = "ToppingData", menuName = "Scriptable Objects/ToppingData", order = 1)]
public class ToppingData : ScriptableObject
{
    [SerializeField] private string toppingName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private float healthRegenBonus;
    [SerializeField] private float glucoseRegenBonus;
    [SerializeField] private float defenseBonus;
    [SerializeField] private float speedBonus;
    [SerializeField] private string toppingGroup;
    public string ToppingName => toppingName;
    public string Description => description;
    public Sprite Icon => icon;

    public global::System.Single HealthRegenBonus { get => healthRegenBonus; set => healthRegenBonus = value; }
    public global::System.Single GlucoseRegenBonus { get => glucoseRegenBonus; set => glucoseRegenBonus = value; }
    public string ToppingGroup => toppingGroup;

    public global::System.Single DefenseBonus { get => defenseBonus; set => defenseBonus = value; }
    public global::System.Single SpeedBonus { get => speedBonus; set => speedBonus = value; }
}
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : EntityData
{
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxGlucose;
    [SerializeField] private float currentGlucose;
    [SerializeField] private float gold;
    [SerializeField] private float experience;
    [ReadOnly] [SerializeField] private int level;
    [SerializeField] private float critRate;
    [SerializeField] private float critBonus;
    [SerializeField] private int currentFloor = 1;
    [SerializeField] private float healCooldown = 0f;
    [SerializeField] private float glucoseCooldown = 0f;

    [Header("Toppings")]
    [SerializeField] private ToppingData[] toppings;
    [SerializeField] private float healthRegenBonus;
    [SerializeField] private float glucoseRegenBonus;
    

    public global::System.Single Gold { get => gold; set => gold = value; }
    public global::System.Single Experience { get => experience; set => experience = value; }
    public global::System.Single CritRate { get => critRate; set => critRate = value; }
    public global::System.Single CritBonus { get => critBonus; set => critBonus = value; }
    public global::System.Single CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public global::System.Single MaxGlucose { get => maxGlucose; set => maxGlucose = value; }
    public global::System.Single CurrentGlucose { get => currentGlucose; set => currentGlucose = value; }
    public global::System.Int32 Level { get => level; }
    public global::System.Int32 CurrentFloor { get => currentFloor; set => currentFloor = value; }
    public global::System.Single HealCooldown { get => healCooldown; set => healCooldown = value; }
    public global::System.Single GlucoseCooldown { get => glucoseCooldown; set => glucoseCooldown = value; }
    public global::System.Single HealthRegenBonus { get => healthRegenBonus; set => healthRegenBonus = value; }
    public global::System.Single GlucoseRegenBonus { get => glucoseRegenBonus; set => glucoseRegenBonus = value; }
    public ToppingData[] Toppings { get => toppings; set => toppings = value; }

    private float thresholdMultiplier = 5000;
    private float thresholdPower = 1.4f;
    public void UpdateLevel()
    {
        this.level = 1 + Mathf.FloorToInt(Mathf.Pow(experience / thresholdMultiplier, 1f / thresholdPower));
    }

    public int GetNextThreshold(float b = -1)
    {
        if (b == -1) b = (float)this.level;

        float res = Mathf.Pow(b, thresholdPower);
        if (b == 0) res = 0;
        return (int)Mathf.Round(thresholdMultiplier * res);
    }

    public void ResetToStarter(PlayerData starterData)
    {
        this.MaxHealth = starterData.MaxHealth;
        this.CurrentHealth = starterData.CurrentHealth;
        this.MaxGlucose = starterData.MaxGlucose;
        this.CurrentGlucose = starterData.CurrentGlucose;
        this.Gold = starterData.Gold;
        this.Experience = starterData.Experience;
        this.CritRate = starterData.CritRate;
        this.CritBonus = starterData.CritBonus;
        this.currentFloor = starterData.CurrentFloor;
        this.healCooldown = starterData.healCooldown;
        this.glucoseCooldown = starterData.glucoseCooldown;
    }


    public void CalculateToppingBonuses()
    {
        float healthRegenBonus = 0f;
        float glucoseRegenBonus = 0f;

        if (toppings != null)
        {
            foreach (var topping in toppings)
            {
                healthRegenBonus += topping.HealthRegenBonus;
                glucoseRegenBonus += topping.GlucoseRegenBonus;
            }
        }

        this.HealthRegenBonus = healthRegenBonus;
        this.GlucoseRegenBonus = glucoseRegenBonus;
    }

    public void AddTopping(ToppingData newTopping)
    {
        if (toppings == null)
        {
            toppings = new ToppingData[0];
        }


        // Add the new topping to the array
        int newSize = toppings.Length + 1;
        ToppingData[] newToppingsArray = new ToppingData[newSize];
        for (int i = 0; i < toppings.Length; i++)
        {
            newToppingsArray[i] = toppings[i];
        }
        newToppingsArray[newSize - 1] = newTopping;
        toppings = newToppingsArray;

        CalculateToppingBonuses();
    }




}

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
    

    public global::System.Single Gold { get => gold; set => gold = value; }
    public global::System.Single Experience { get => experience; set => experience = value; }
    public global::System.Single CritRate { get => critRate; set => critRate = value; }
    public global::System.Single CritBonus { get => critBonus; set => critBonus = value; }
    public global::System.Single CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public global::System.Single MaxGlucose { get => maxGlucose; set => maxGlucose = value; }
    public global::System.Single CurrentGlucose { get => currentGlucose; set => currentGlucose = value; }
    public global::System.Int32 Level { get => level; }
    public global::System.Int32 CurrentFloor { get => currentFloor; set => currentFloor = value; }

    private float thresholdMultiplier = 1000;
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
    }




}

using UnityEngine;


public enum SpawnChance
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class EnemySpawnEntry
{
    public EnemyData enemyData;
    public SpawnChance spawnChance;
    public float GetSpawnChanceValue()
    {
        switch (spawnChance)
        {
            case SpawnChance.Common: return 0.5f;
            case SpawnChance.Uncommon: return 0.3f;
            case SpawnChance.Rare: return 0.15f;
            case SpawnChance.Epic: return 0.04f;
            case SpawnChance.Legendary: return 0.01f;
            default: return 0f;
        }
    }
}

[CreateAssetMenu(fileName = "EnemySpawns", menuName = "Scriptable Objects/EnemySpawns")]
public class EnemySpawns : ScriptableObject
{
    [SerializeField] private EnemySpawnEntry[] enemySpawnEntries;

    public EnemySpawnEntry[] DropEntries { get => enemySpawnEntries; set => enemySpawnEntries = value; }

    public EnemyData GetRandomEnemy()
    {
        if (enemySpawnEntries == null || enemySpawnEntries.Length == 0)
        {
            Debugger.LogWarning("No drop entries defined in EnemySpawns.", type: DebugType.Items);
            return null;
        }

        // Calculate total weight based on drop chances
        float totalWeight = 0f;
        foreach (var entry in enemySpawnEntries)
        {
            totalWeight += entry.GetSpawnChanceValue();
        }

        // Get a random value between 0 and total weight
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        // Determine which item to drop based on the random value
        foreach (var entry in enemySpawnEntries)
        {
            cumulativeWeight += entry.GetSpawnChanceValue();
            if (randomValue <= cumulativeWeight)
            {
                return entry.enemyData;
            }
        }

        // Fallback in case of rounding errors
        return enemySpawnEntries[enemySpawnEntries.Length - 1].enemyData;
    }
}

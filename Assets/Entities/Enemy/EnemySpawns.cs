using UnityEngine;



[System.Serializable]
public class EnemySpawnEntry
{
    public EnemyData enemyData;
    public float spawnWeight = 1f;
   
}

[CreateAssetMenu(fileName = "EnemySpawns", menuName = "Scriptable Objects/EnemySpawns")]
public class EnemySpawns : ScriptableObject
{
    [SerializeField] private EnemySpawnEntry[] tier1Enemies;
    [SerializeField] private EnemySpawnEntry[] tier2Enemies;
    [SerializeField] private EnemySpawnEntry[] tier3Enemies;
    [SerializeField] private EnemySpawnEntry[] tier4Enemies;
    [SerializeField] private EnemySpawnEntry[] tier5Enemies;
    [SerializeField] private EnemySpawnEntry[] tier6Enemies;


    public EnemyData GetRandomEnemy(int tier)
    {
        // Collect all enemy entries for tiers up to the specified tier
        var allEntries = new System.Collections.Generic.List<EnemySpawnEntry>();
        
        if (tier >= 1 && tier1Enemies != null) allEntries.AddRange(tier1Enemies);
        if (tier >= 2 && tier2Enemies != null) allEntries.AddRange(tier2Enemies);
        if (tier >= 3 && tier3Enemies != null) allEntries.AddRange(tier3Enemies);
        if (tier >= 4 && tier4Enemies != null) allEntries.AddRange(tier4Enemies);
        if (tier >= 5 && tier5Enemies != null) allEntries.AddRange(tier5Enemies);
        if (tier >= 6 && tier6Enemies != null) allEntries.AddRange(tier6Enemies);
        
        if (allEntries.Count == 0)
        {
            Debugger.LogWarning("No enemy entries defined for tier " + tier + ".", type: DebugType.Items);
            return null;
        }

        // Calculate total weight
        float totalWeight = 0f;
        foreach (var entry in allEntries)
        {
            totalWeight += entry.spawnWeight;
        }

        // Get random enemy based on weighted distribution
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var entry in allEntries)
        {
            cumulativeWeight += entry.spawnWeight;
            if (randomValue <= cumulativeWeight)
            {
                return entry.enemyData;
            }
        }

        // Fallback in case of rounding errors
        return allEntries[allEntries.Count - 1].enemyData;
    }
}

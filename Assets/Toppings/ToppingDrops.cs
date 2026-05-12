using UnityEngine;



[System.Serializable]
public class ToppingDropEntry
{
    public ToppingData item;
    public float dropWeight = 1f;
}

[CreateAssetMenu(fileName = "ToppingDrops", menuName = "Scriptable Objects/ToppingDrops")]
public class ToppingDrops : ScriptableObject
{
    [SerializeField] private ToppingDropEntry[] tier1Drops;
    [SerializeField] private ToppingDropEntry[] tier2Drops;
    [SerializeField] private ToppingDropEntry[] tier3Drops;
    [SerializeField] private ToppingDropEntry[] tier4Drops;
    [SerializeField] private ToppingDropEntry[] tier5Drops;
    [SerializeField] private ToppingDropEntry[] tier6Drops;

    public ToppingDropEntry[] GetRandomDrops(int tier, int count)
    {
        var drops = new ToppingDropEntry[count];
        for (int i = 0; i < count; i++)
        {
            drops[i] = GetRandomDrop(tier);

            // ensure no other drops have same topping group
            while (i > 0 && drops[i] != null)
            {
                bool hasDuplicateGroup = false;
                for (int j = 0; j < i; j++)
                {
                    if (drops[j] != null && drops[j].item.ToppingGroup == drops[i].item.ToppingGroup)
                    {
                        hasDuplicateGroup = true;
                        break;
                    }
                }

                if (!hasDuplicateGroup)
                    break;

                 drops[i] = GetRandomDrop(tier);
            }

             // If we couldn't find a non-duplicate drop, just accept the duplicate
             if (drops[i] == null)
            {
                drops[i] = GetRandomDrop(tier);
            }
        }    

        return drops;
        
    }
  

    public ToppingDropEntry GetRandomDrop(int tier)
    {
        var allEntries = new System.Collections.Generic.List<ToppingDropEntry>();
        
        if (tier >= 1 && tier1Drops != null) allEntries.AddRange(tier1Drops);
        if (tier >= 2 && tier2Drops != null) allEntries.AddRange(tier2Drops);
        if (tier >= 3 && tier3Drops != null) allEntries.AddRange(tier3Drops);
        if (tier >= 4 && tier4Drops != null) allEntries.AddRange(tier4Drops);
        if (tier >= 5 && tier5Drops != null) allEntries.AddRange(tier5Drops);
        if (tier >= 6 && tier6Drops != null) allEntries.AddRange(tier6Drops);
        
        if (allEntries.Count == 0)
        {
            Debugger.LogWarning("No topping drop entries defined for tier " + tier + ".", type: DebugType.Items);
            return null;
        }

        // Calculate total weight based on drop chances
        float totalWeight = 0f;
        foreach (var entry in allEntries)
        {
            totalWeight += entry.dropWeight;
        }

        // Get a random value between 0 and total weight
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        // Determine which item to drop based on the random value
        foreach (var entry in allEntries)
        {
            cumulativeWeight += entry.dropWeight;
            if (randomValue <= cumulativeWeight)
            {
                return entry;
            }
        }

        // Fallback in case of rounding errors
        return allEntries[allEntries.Count - 1];
    }
}

using UnityEngine;


public enum DropChance
{
    Common,
    Rare,
    Epic,
}

[System.Serializable]
public class ItemDropEntry
{
    public ItemData item;
    public DropChance dropChance;
    public int count = 1;
    public bool isGuaranteed = false;
    public float GetDropChanceValue()
    {
        switch (dropChance)
        {
            case DropChance.Common: return 1f;
            case DropChance.Rare: return 0.5f;
            case DropChance.Epic: return 0.25f;
            default: return 0f;
        }
    }
}

[CreateAssetMenu(fileName = "ItemDrops", menuName = "Scriptable Objects/ItemDrops")]
public class ItemDrops : ScriptableObject
{
    [SerializeField] private ItemDropEntry[] tier1Drops;
    [SerializeField] private ItemDropEntry[] tier2Drops;
    [SerializeField] private ItemDropEntry[] tier3Drops;
    [SerializeField] private ItemDropEntry[] tier4Drops;
    [SerializeField] private ItemDropEntry[] tier5Drops;
    [SerializeField] private ItemDropEntry[] tier6Drops;

    public ItemDropEntry[] GetRandomDrops(int tier, int count)
    {
        var drops = new ItemDropEntry[count];
        for (int i = 0; i < count; i++)
        {
            drops[i] = GetRandomDrop(tier);
        }

        // ensure at least one guaranteed drop if any entries are marked as guaranteed
        bool hasGuaranteed = false;
        if (tier >= 1 && tier1Drops != null) hasGuaranteed |= System.Array.Exists(tier1Drops, entry => entry.isGuaranteed);
        if (tier >= 2 && tier2Drops != null) hasGuaranteed |= System.Array.Exists(tier2Drops, entry => entry.isGuaranteed);
        if (tier >= 3 && tier3Drops != null) hasGuaranteed |= System.Array.Exists(tier3Drops, entry => entry.isGuaranteed);
        if (tier >= 4 && tier4Drops != null) hasGuaranteed |= System.Array.Exists(tier4Drops, entry => entry.isGuaranteed);
        if (tier >= 5 && tier5Drops != null) hasGuaranteed |= System.Array.Exists(tier5Drops, entry => entry.isGuaranteed);
        if (tier >= 6 && tier6Drops != null) hasGuaranteed |= System.Array.Exists(tier6Drops, entry => entry.isGuaranteed);

        if (hasGuaranteed && !System.Array.Exists(drops, entry => entry != null && entry.isGuaranteed))
        {
            int replaceIndex = Random.Range(0, drops.Length);
            drops[replaceIndex] = GetRandomGuaranteedDrop(tier);
        }

        return drops;
        
    }

    public ItemDropEntry GetRandomGuaranteedDrop(int tier)
    {
        var guaranteedEntries = new System.Collections.Generic.List<ItemDropEntry>();
        
        if (tier >= 1 && tier1Drops != null) guaranteedEntries.AddRange(System.Array.FindAll(tier1Drops, entry => entry.isGuaranteed));
        if (tier >= 2 && tier2Drops != null) guaranteedEntries.AddRange(System.Array.FindAll(tier2Drops, entry => entry.isGuaranteed));
        if (tier >= 3 && tier3Drops != null) guaranteedEntries.AddRange(System.Array.FindAll(tier3Drops, entry => entry.isGuaranteed));
        if (tier >= 4 && tier4Drops != null) guaranteedEntries.AddRange(System.Array.FindAll(tier4Drops, entry => entry.isGuaranteed));
        if (tier >= 5 && tier5Drops != null) guaranteedEntries.AddRange(System.Array.FindAll(tier5Drops, entry => entry.isGuaranteed));
        if (tier >= 6 && tier6Drops != null) guaranteedEntries.AddRange(System.Array.FindAll(tier6Drops, entry => entry.isGuaranteed));
        
        if (guaranteedEntries.Count == 0)
        {
            Debugger.LogWarning("No guaranteed item drop entries defined for tier " + tier + ".", type: DebugType.Items);
            return null;
        }

        int randomIndex = Random.Range(0, guaranteedEntries.Count);
        return guaranteedEntries[randomIndex];
    }
  

    public ItemDropEntry GetRandomDrop(int tier)
    {
        var allEntries = new System.Collections.Generic.List<ItemDropEntry>();
        
        if (tier >= 1 && tier1Drops != null) allEntries.AddRange(tier1Drops);
        if (tier >= 2 && tier2Drops != null) allEntries.AddRange(tier2Drops);
        if (tier >= 3 && tier3Drops != null) allEntries.AddRange(tier3Drops);
        if (tier >= 4 && tier4Drops != null) allEntries.AddRange(tier4Drops);
        if (tier >= 5 && tier5Drops != null) allEntries.AddRange(tier5Drops);
        if (tier >= 6 && tier6Drops != null) allEntries.AddRange(tier6Drops);
        
        if (allEntries.Count == 0)
        {
            Debugger.LogWarning("No item drop entries defined for tier " + tier + ".", type: DebugType.Items);
            return null;
        }

        // Calculate total weight based on drop chances
        float totalWeight = 0f;
        foreach (var entry in allEntries)
        {
            totalWeight += entry.GetDropChanceValue();
        }

        // Get a random value between 0 and total weight
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        // Determine which item to drop based on the random value
        foreach (var entry in allEntries)
        {
            cumulativeWeight += entry.GetDropChanceValue();
            if (randomValue <= cumulativeWeight)
            {
                return entry;
            }
        }

        // Fallback in case of rounding errors
        return allEntries[allEntries.Count - 1];
    }
}

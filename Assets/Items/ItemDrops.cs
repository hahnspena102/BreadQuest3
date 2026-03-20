using UnityEngine;


public enum DropChance
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class ItemDropEntry
{
    public ItemData item;
    public DropChance dropChance;
    public float GetDropChanceValue()
    {
        switch (dropChance)
        {
            case DropChance.Common: return 0.5f;
            case DropChance.Uncommon: return 0.3f;
            case DropChance.Rare: return 0.15f;
            case DropChance.Epic: return 0.04f;
            case DropChance.Legendary: return 0.01f;
            default: return 0f;
        }
    }
}

[CreateAssetMenu(fileName = "ItemDrops", menuName = "Scriptable Objects/ItemDrops")]
public class ItemDrops : ScriptableObject
{
    [SerializeField] private ItemDropEntry[] dropEntries;

    public ItemDropEntry[] DropEntries { get => dropEntries; set => dropEntries = value; }

    public ItemData GetRandomDrop()
    {
        if (dropEntries == null || dropEntries.Length == 0)
        {
            Debugger.LogWarning("No drop entries defined in ItemDrops.", type: DebugType.Items);
            return null;
        }

        // Calculate total weight based on drop chances
        float totalWeight = 0f;
        foreach (var entry in dropEntries)
        {
            totalWeight += entry.GetDropChanceValue();
        }

        // Get a random value between 0 and total weight
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        // Determine which item to drop based on the random value
        foreach (var entry in dropEntries)
        {
            cumulativeWeight += entry.GetDropChanceValue();
            if (randomValue <= cumulativeWeight)
            {
                return entry.item;
            }
        }

        // Fallback in case of rounding errors
        return dropEntries[dropEntries.Length - 1].item;
    }
}

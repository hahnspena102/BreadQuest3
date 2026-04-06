using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Objects/Inventory")]
public class Inventory : ScriptableObject
{
    [SerializeField] private int capacity;
    [SerializeField] private Item[] items;
    [SerializeField] private int currentItemIndex = 0;
    [SerializeField] private Item equippedItem;
    
   

    public int Capacity { get => capacity; set => capacity = value; }
    public int CurrentItemIndex { get => currentItemIndex; set => currentItemIndex = value; }
    public Item EquippedItem { get => equippedItem; set => equippedItem = value; }
    public Item[] Items { get => items; set => items = value; }

    public void NormalizeItemTypes()
    {
        if (items == null)
        {
            return;
        }

        for (int i = 0; i < items.Length; i++)
        {
            Item originalItem = items[i];
            if (originalItem == null)
            {
                continue;
            }

            ItemData data = originalItem.ItemData;
            if (data == null)
            {
                items[i] = null;
                continue;
            }

            Item normalizedItem = ItemFactory.Clone(originalItem);
            if (normalizedItem != null)
            {
                items[i] = normalizedItem;
            }
        }

        if (currentItemIndex >= 0 && currentItemIndex < items.Length)
        {
            equippedItem = items[currentItemIndex];
        }
        else
        {
            equippedItem = null;
        }
    }

    public Item GetItemAtIndex(int index)
    {
        if (items != null && index >= 0 && index < items.Length)
        {
            Item item = items[index];
            return IsSlotEmpty(item) ? null : item;
        }
        return null;
    }

    public void SetItemAtIndex(int index, Item item)
    {
        if (items != null && index >= 0 && index < items.Length)
        {
            items[index] = item;
        }
    }

    public void CycleItem(float direction)
    {
        if (items == null)
        {
            return;
        }

        if (items.Length == 0) return;

        if (currentItemIndex == -1) currentItemIndex = 0;

        if (direction > 0)
        {
            currentItemIndex = (currentItemIndex + 1) % items.Length;
        }
        else if (direction < 0)
        {
            currentItemIndex = (currentItemIndex - 1 + items.Length) % items.Length;
        }

        equippedItem = items[currentItemIndex];
    }

    public void CycleTo(int index)
    {
        if (items == null)
        {
            return;
        }

        if (items.Length == 0) return;
        if (index >= 0 && index < items.Length)
        {
            currentItemIndex = index;
            equippedItem = items[currentItemIndex];
        }
    }

    private bool IsFull()
    {
        if (items == null)
        {
            return false;
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (IsSlotEmpty(items[i]))
                return false;
        }

        return true;
    }

    private bool IsSlotEmpty(Item item)
    {
        return item == null || item.ItemData == null || item.Count <= 0;
    }

    private int GetMaxStackSize(ItemData itemData)
    {
        if (itemData == null)
        {
            return 1;
        }

        return Mathf.Max(1, itemData.MaxStackSize);
    }

    public int AddItem(Item sourceItem)
    {
        if (sourceItem == null || sourceItem.ItemData == null)
        {
            Debug.LogWarning("Trying to add an item with no ItemData!");
            return 0;
        }

        if (items == null || items.Length == 0)
        {
            return sourceItem.Count;
        }

        int remaining = Mathf.Max(1, sourceItem.Count);
        ItemData sourceData = sourceItem.ItemData;
        int maxStack = GetMaxStackSize(sourceData);

        // Fill matching stacks first.
        for (int i = 0; i < items.Length && remaining > 0; i++)
        {
            Item existingItem = items[i];
            if (IsSlotEmpty(existingItem) || existingItem.ItemData != sourceData)
            {
                continue;
            }

            int existingCount = Mathf.Max(1, existingItem.Count);
            int availableSpace = maxStack - existingCount;
            if (availableSpace <= 0)
            {
                continue;
            }

            int addAmount = Mathf.Min(availableSpace, remaining);
            existingItem.Count = existingCount + addAmount;
            remaining -= addAmount;
        }

        // Then place into empty slots.
        for (int i = 0; i < items.Length && remaining > 0; i++)
        {
            if (!IsSlotEmpty(items[i]))
            {
                continue;
            }

            Item newStack = ItemFactory.Clone(sourceItem);
            if (newStack == null)
            {
                continue;
            }

            int stackCount = Mathf.Min(maxStack, remaining);
            newStack.Count = stackCount;
            items[i] = newStack;
            remaining -= stackCount;
        }

        return remaining;
    }

    public bool AddItem(DroppedItem droppedItem)
    {
        if (droppedItem == null || droppedItem.Item == null)
        {
            Debug.LogWarning("Trying to add an item with no ItemData!");
            return false;
        }

        int remaining = AddItem(droppedItem.Item);
        if (remaining <= 0)
        {
            return true;
        }

        droppedItem.Item.Count = remaining;
        if (IsFull())
        {
            Debug.LogWarning("Inventory is full.");
        }

        return false;
    }

    public int NextEmptySlot()
    {
        if (items == null)
        {
            return -1;
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (IsSlotEmpty(items[i]))
                return i;
        }
        return -1; 
    }

    public void ResetToStarter(Inventory starterInventory)
    {
        if (starterInventory == null || starterInventory.Items == null)
        {
            Debug.LogWarning("Trying to reset inventory with invalid starter inventory.");
            return;
        }

        capacity = starterInventory.Capacity;
        items = new Item[capacity];
        for (int i = 0; i < Mathf.Min(items.Length, starterInventory.Items.Length); i++)
        {
            Item starterItem = starterInventory.Items[i];
            if (starterItem != null)
            {
                items[i] = ItemFactory.Clone(starterItem);
            }
        }
        currentItemIndex = 0;
        equippedItem = items.Length > 0 ? items[0] : null;
    }


}

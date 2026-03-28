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
            return items[index];
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
            if (items[i] == null)
                return false;
        }

        return true;
    }
    public void AddItem(DroppedItem droppedItem)
    {
        if (droppedItem == null || droppedItem.Item == null)
        {
            Debug.LogWarning("Trying to add an item with no ItemData!");
            return;
        }

        if (IsFull())
        {
            Debug.LogWarning("Inventory is full.");
            return;
        }

        Item newItem = ItemFactory.Clone(droppedItem.Item);
        if (newItem == null || newItem.ItemData == null)
        {
            Debug.LogWarning("Trying to add an item with no ItemData!");
            return;
        }

        // Find the first empty slot
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = newItem;
                break;
            }
        }
    }

    public int NextEmptySlot()
    {
        if (items == null)
        {
            return -1;
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
                return i;
        }
        return -1; 
    }


}

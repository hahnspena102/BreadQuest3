using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Objects/Inventory")]
public class Inventory : ScriptableObject
{
    [SerializeField] private int capacity;
    [SerializeField] private ItemData[] itemDatas;
    [SerializeField] private int currentItemIndex = 0;
    [SerializeField] private ItemData equippedItemData;
    
   

    public int Capacity { get => capacity; set => capacity = value; }
    public ItemData[] ItemDatas { get => itemDatas; set => itemDatas = value; }
    public ItemData EquippedItemData { get => equippedItemData; set => equippedItemData = value; }
    public int CurrentItemIndex { get => currentItemIndex; set => currentItemIndex = value; }
    public ItemData GetItemAtIndex(int index)
    {
        if (index >= 0 && index < itemDatas.Length)
        {
            return itemDatas[index];
        }
        return null;
    }

    public void SetItemAtIndex(int index, ItemData itemData)
    {
        if (index >= 0 && index < itemDatas.Length)
        {
            itemDatas[index] = itemData;
        }
    }

    public void CycleItem(float direction)
    {
        if (itemDatas.Length == 0) return;

        if (currentItemIndex == -1) currentItemIndex = 0;

        if (direction > 0)
        {
            currentItemIndex = (currentItemIndex + 1) % itemDatas.Length;
        }
        else if (direction < 0)
        {
            currentItemIndex = (currentItemIndex - 1 + itemDatas.Length) % itemDatas.Length;
        }

        equippedItemData = itemDatas[currentItemIndex];

    }

    public void CycleTo(int index)
    {
        if (itemDatas.Length == 0) return;
        if (index >= 0 && index < itemDatas.Length)
        {
            currentItemIndex = index;
            equippedItemData = itemDatas[currentItemIndex];
 
        }
    }

    private bool IsFull()
    {
        for (int i = 0; i < itemDatas.Length; i++)
        {
            if (itemDatas[i] == null)
                return false;
        }

        return true;
    }
    public void AddItem(Item item)
    {
        if (item.ItemData == null)
        {
            Debug.LogWarning("Trying to add an item with no ItemData!");
            return;
        }

        if (IsFull())
        {
            Debug.LogWarning("Inventory is full.");
            return;
        }

        ItemData newItemData = item.ItemData;

        // Find the first empty slot
        for (int i = 0; i < itemDatas.Length; i++)
        {
            if (itemDatas[i] == null)
            {
                itemDatas[i] = newItemData;
                break;
            }
        }
    }

    public int NextEmptySlot()
    {
        for (int i = 0; i < itemDatas.Length; i++)
        {
            if (itemDatas[i] == null)
                return i;
        }
        return -1; 
    }


}

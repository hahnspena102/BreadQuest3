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
        if (equippedItemData != null)
        {
            Debug.Log("Cycled to Item: " + equippedItemData.ItemName);
        } else {
            Debug.Log("No item equipped.");
        }
    }

    public void CycleTo(int index)
    {
        if (itemDatas.Length == 0) return;
        if (index >= 0 && index < itemDatas.Length)
        {
            currentItemIndex = index;
            equippedItemData = itemDatas[currentItemIndex];
            if (equippedItemData != null)
            {
                Debug.Log("Cycled to Item: " + equippedItemData.ItemName);
            } else {
                Debug.Log("No item equipped.");
            }
        }
    }


}

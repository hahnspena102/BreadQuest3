using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Objects/Inventory")]
public class Inventory : ScriptableObject
{
    [SerializeField] private int capacity;
    [SerializeField] private Item[] items;
    [SerializeField] private int currentItemIndex = 0;
    [SerializeField] private Item equippedItem;
    
   

    public int Capacity { get => capacity; set => capacity = value; }
    public Item[] Items { get => items; set => items = value; }
    public Item EquippedItem { get => equippedItem; set => equippedItem = value; }
    public int CurrentItemIndex { get => currentItemIndex; set => currentItemIndex = value; }
    public Item GetItemAtIndex(int index)
    {
        if (index >= 0 && index < items.Length)
        {
            return items[index];
        }
        return null;
    }

    public void CycleItem(float direction)
    {
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
        if (equippedItem != null)
        {
            Debug.Log("Cycled to Item: " + equippedItem.ItemName);
        } else {
            Debug.Log("No item equipped.");
        }
    }

    public void CycleTo(int index)
    {
        if (items.Length == 0) return;
        if (index >= 0 && index < items.Length)
        {
            currentItemIndex = index;
            equippedItem = items[currentItemIndex];
            if (equippedItem != null)
            {
                Debug.Log("Cycled to Item: " + equippedItem.ItemName);
            } else {
                Debug.Log("No item equipped.");
            }
        }
    }


}

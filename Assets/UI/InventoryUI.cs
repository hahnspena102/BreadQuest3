using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Components")]


    [Header("Inventory Data")]
    [SerializeField] private Inventory inventory;

    [Header("Prefabs")]
    [SerializeField] private GameObject itemSlotPrefab;

    private void Awake()
    {
        PopulateInventory();
    }

    private void Update()
    {
        UpdateInventoryUI();
    }
    

    private void PopulateInventory()
    {
        for (int i = 0 ; i < inventory.Capacity; i++)
        {
            GameObject itemSlot = Instantiate(itemSlotPrefab, transform);
        }
    }

    private void UpdateInventoryUI()
    {
        for (int i = 0; i < inventory.Capacity; i++)
        {
            Item item = inventory.GetItemAtIndex(i);
            ItemSlotUI itemSlotUI = transform.GetChild(i).GetComponent<ItemSlotUI>();
            if (itemSlotUI != null)
            {
                itemSlotUI.SetItem(item);
            }

            if (i == inventory.CurrentItemIndex)
            {
                itemSlotUI.HighlightSlot(true);
            }
            else
            {
                itemSlotUI.HighlightSlot(false);
            }
        }
    }
}

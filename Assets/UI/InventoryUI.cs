using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private Player player;

    [SerializeField] private CanvasGroup inventoryCanvasGroup;
    [SerializeField] private InventoryUISlot[] inventorySlots;

    private Item[] lastItems;

    void Start()
    {
        player = FindFirstObjectByType<Player>();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].SlotIndex = i;
        }

        ForceRefresh();
    }

    void Update()
    {
        if (player == null) return;

        bool visible = player.IsInInventory;

        inventoryCanvasGroup.alpha = visible ? 1f : 0f;
        inventoryCanvasGroup.interactable = visible;
        inventoryCanvasGroup.blocksRaycasts = visible;

        if (InventoryChanged())
        {
            UpdateInventorySlots();
        }
    }

    void ForceRefresh()
    {
        UpdateInventorySlots();
        CacheItems();
    }

    void UpdateInventorySlots()
    {
        var items = player.Inventory.Items;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < items.Length)
            {
                inventorySlots[i].SetItem(items[i]);
            }
            else
            {
                inventorySlots[i].SetItem(null);
            }
        }

        CacheItems();
    }

    void CacheItems()
    {
        var items = player.Inventory.Items;

        lastItems = new Item[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            lastItems[i] = items[i];
        }
    }

    bool InventoryChanged()
    {
        var items = player.Inventory.Items;

        if (lastItems == null || lastItems.Length != items.Length)
        {
            return true;
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (lastItems[i] != items[i])
            {
                return true;
            }
        }

        return false;
    }
}
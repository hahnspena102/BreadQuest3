using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class InventoryUI : MonoBehaviour
{
    private Player player;
    [SerializeField] private CanvasGroup inventoryCanvasGroup;
    [SerializeField] private InventoryUISlot[] inventorySlots;

    void Start() {
        player = FindFirstObjectByType<Player>();

        for (int i = 0; i < inventorySlots.Length; i++) {
            inventorySlots[i].SlotIndex = i;
        }
    }

    void Update()
    {
        if (player.IsInInventory)
        {
            inventoryCanvasGroup.alpha = 1f;
            inventoryCanvasGroup.interactable = true;
            inventoryCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            inventoryCanvasGroup.alpha = 0f;
            inventoryCanvasGroup.interactable = false;
            inventoryCanvasGroup.blocksRaycasts = false;
        }

        UpdateInventorySlots();
    }

    void UpdateInventorySlots() {
        for (int i = 0; i < inventorySlots.Length; i++) {
            if (i < player.Inventory.Items.Length) {
                inventorySlots[i].SetItem(player.Inventory.Items[i]);
            } else {
                inventorySlots[i].SetItem(null);
            }
        }
    }
}
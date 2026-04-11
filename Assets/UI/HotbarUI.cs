using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class HotbarUI : MonoBehaviour
{
    [Header("Components")]


    [Header("Inventory Data")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField]private Item previouslyHighlightedItem;

    [Header("Prefabs")]
    [SerializeField] private GameObject itemSlotPrefab;
    private Coroutine currentlyDisplayedItemNameCoroutine;

    private void Awake()
    {
        itemNameText.text = "";
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

        if (inventory.EquippedItem != previouslyHighlightedItem)
        {
            DisplayItemName(inventory.EquippedItem);
            previouslyHighlightedItem = inventory.EquippedItem;
        }
    }

    private void DisplayItemName(Item item)
    {
        
        if (item == null || item.ItemData == null)
        {
            itemNameText.text = "";
            return;
        }
        itemNameText.text = item.ItemData.ItemName;
        if (currentlyDisplayedItemNameCoroutine != null)
        {
            StopCoroutine(currentlyDisplayedItemNameCoroutine);
        }
        itemNameText.color = Color.white;
        currentlyDisplayedItemNameCoroutine = StartCoroutine(FadeItemName());
    }

    IEnumerator FadeItemName()
    {
        float duration = 1f;
        float elapsed = 0f;
        Color originalColor = itemNameText.color;
        yield return new WaitForSeconds(1f); 
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            itemNameText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        itemNameText.text = "";
        itemNameText.color = originalColor;
    }
}

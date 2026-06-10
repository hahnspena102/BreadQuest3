using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUISlot : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image itemSprite;
    [SerializeField] private Image highlightBorder;
    [SerializeField] private Item item;
    [SerializeField] private TextMeshProUGUI quantityText;
    [ReadOnly] [SerializeField] private int slotIndex;
    private Player player;
    private UseItem useItem;

    public global::System.Int32 SlotIndex { get => slotIndex; set => slotIndex = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player>();
        useItem = FindFirstObjectByType<UseItem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.Inventory.GetSelectedItem() == item)
        {
            HighlightSlot(true);
        }
        else
        {
            HighlightSlot(false);
        }
    }

    public void SetItem(Item newItem)
    {
        item = newItem;

        if (item == null)
        {
            itemSprite.sprite = null;
            itemSprite.enabled = false;
            quantityText.text = "";

            return;
        } else if (item.ItemData == null)
        {
            itemSprite.sprite = null;
            itemSprite.enabled = false;
            quantityText.text = "";
   
            return;
        }
        
        if (itemSprite != null)
        {
            itemSprite.sprite = item.ItemData.ItemSprite;
            itemSprite.enabled = true;
        }
        quantityText.text = item.Count > 1 ? item.Count.ToString() : "";

     
    

    }

    public void HighlightSlot(bool highlight)
    {
        if (highlightBorder != null)
        {
            highlightBorder.enabled = highlight;
        }
        itemSprite.rectTransform.localScale = highlight ? Vector3.one * 1.4f : Vector3.one;
    }

    public void SelectItem()
    {
        player.Inventory.CycleTo(slotIndex);
    }

    
}

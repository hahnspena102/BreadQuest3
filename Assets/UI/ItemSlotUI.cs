using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image itemSprite;
    [SerializeField] private Image highlightBorder;
    [SerializeField] private Item item;
    [SerializeField] private TextMeshProUGUI quantityText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
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
}

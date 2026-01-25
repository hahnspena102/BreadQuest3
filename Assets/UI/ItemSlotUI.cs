using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image itemSprite;
    [SerializeField] private Image highlightBorder;
    [SerializeField] private ItemData itemData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetItem(ItemData newItem)
    {
        itemData = newItem;
        if (itemData == null)
        {
            itemSprite.sprite = null;
            itemSprite.enabled = false;
            return;
        }
        if (itemSprite != null && itemData != null)
        {
            itemSprite.sprite = itemData.ItemSprite;
            itemSprite.enabled = true;
        }
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

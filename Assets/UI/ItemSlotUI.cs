using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image itemIcon;
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
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            return;
        }
        if (itemIcon != null && itemData != null)
        {
            itemIcon.sprite = itemData.Icon;
            itemIcon.enabled = true;
        }
    }

    public void HighlightSlot(bool highlight)
    {
        if (highlightBorder != null)
        {
            highlightBorder.enabled = highlight;
        }
        itemIcon.rectTransform.localScale = highlight ? Vector3.one * 1.4f : Vector3.one;
    }
}

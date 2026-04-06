using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [SerializeField] private Item item;

    public Item Item
    {
        get => item;
        set
        {
            item = value;
            RefreshVisuals();
        }
    }
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        RefreshVisuals();
    }

    public void RefreshVisuals()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        ItemData itemData = item != null ? item.ItemData : null;

        gameObject.name = itemData != null ? itemData.ItemName : "Item";
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = itemData != null ? itemData.ItemSprite : null;
        }
    }

    // Update is called once per frame
    void Update()
    {
     
    }

}

using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [SerializeField] private Item item;

    public Item Item { get => item; set => item = value; }
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ItemData itemData = item != null ? item.ItemData : null;

        gameObject.name = itemData != null ? itemData.ItemName : "Item";
        if (spriteRenderer != null && itemData != null && itemData.ItemSprite != null)
        {
            spriteRenderer.sprite = itemData.ItemSprite;
        }

    }

    // Update is called once per frame
    void Update()
    {
     
    }

}

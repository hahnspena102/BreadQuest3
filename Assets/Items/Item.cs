using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    public ItemData ItemData { get => itemData; set => itemData = value; }
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

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

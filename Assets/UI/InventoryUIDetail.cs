using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIDetail : MonoBehaviour
{
    private Player player;
    private Item currentItem;

    public Image itemSprite;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;

     void Start() {
        player = FindFirstObjectByType<Player>();
    }


    public void Update()
    {
        currentItem = player.Inventory.GetSelectedItem();

        if (currentItem != null && currentItem.ItemData != null)
        {
            itemSprite.color = new Color(1f, 1f, 1f, 1f);
            itemSprite.sprite = currentItem.ItemData.ItemSprite;
            itemNameText.text = currentItem.ItemData.ItemName;
            if (currentItem.Count > 1)
            {
                itemNameText.text += $" (x{currentItem.Count})";
            }
            itemDescriptionText.text = currentItem.ItemData.GetFullDescription();
        }
        else
        {

            itemSprite.color = new Color(1f, 1f, 1f, 0f);
            itemNameText.text = "";
            itemDescriptionText.text = "";
        }
    }
    
}

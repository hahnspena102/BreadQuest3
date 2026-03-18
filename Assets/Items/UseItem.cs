using UnityEngine;

public class UseItem : MonoBehaviour
{
    private Player player;
    private Inventory inventory;
    private ItemData equippedItemData;
    
    void Start()
    {
        player = GetComponentInParent<Player>();
        inventory = player.Inventory;
    }
    
    void Update()
    {
       if (inventory == null)
        {
            return;
        }

        equippedItemData = inventory.GetItemAtIndex(inventory.CurrentItemIndex);

        if (equippedItemData != null)
        {
            if (equippedItemData is MeleeData)
            {
                MeleeWeapon();
            } else if (equippedItemData is MagicData)
            {
                MagicWeapon();
            }
        }
    }
    public void MeleeWeapon()
    {
        if (Input.GetMouseButtonDown(0))
        {    
            Vector2 direction = player.WorldPointPosition - (Vector2)player.transform.position;

            Debug.Log("Attacking with " + equippedItemData.name + " in direction: " + direction.normalized);
            

            player.MeleeAttack(direction.normalized);
        }
    }

    public void MagicWeapon()
    {
       if (Input.GetMouseButtonDown(0))
        {    
            Vector2 direction = player.WorldPointPosition - (Vector2)player.transform.position;

            Debug.Log("Attacking with " + equippedItemData.name + " in direction: " + direction.normalized);


            player.MagicAttack(direction.normalized);
        }
    }
}


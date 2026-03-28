using UnityEngine;

public class UseItem : MonoBehaviour
{
    private Player player;
    private Inventory inventory;
    private Item equippedItem;
    private ItemAnimator itemAnimator;
    [SerializeField] private GameObject itemSpriteHolder;
    
    void Start()
    {
        player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            inventory = player.Inventory;
            if (inventory != null)
            {
                inventory.NormalizeItemTypes();
            }
        }
        itemAnimator = GetComponentInChildren<ItemAnimator>();

    }
    
    void Update()
    {
       if (inventory == null)
        {
            return;
        }

        equippedItem = inventory.GetItemAtIndex(inventory.CurrentItemIndex);

        if (equippedItem != null)
        {   
            ItemData equippedItemData = equippedItem.ItemData;
            if (equippedItemData == null)
            {
                return;
            }

        
            if (equippedItemData is MeleeData meleeData)
            {
                MeleeWeapon(meleeData.MeleeScale, meleeData.MeleeSpeed);
            } else if (equippedItemData is MagicData)
            {
                MagicWeapon();
            } else if (equippedItemData is PotionData potionData)
            {
                UsePotion(potionData);
            }
        }
    }
    public void MeleeWeapon(float scale, float speed)
    {
        if (Input.GetMouseButtonDown(0))
        {    
            Vector2 direction = player.WorldPointPosition - (Vector2)player.transform.position;

            if (player.IsAttacking) return;
            if (direction == Vector2.zero) return;
            

            player.IsAttacking = true;

            // ANIMATION
            Debug.Log("Playing melee animation with scale: " + scale + " and speed: " + speed);
            itemAnimator.SetSpeed(speed);
            player.Animator.speed = speed;
            itemAnimator.transform.localScale = new Vector3(scale, scale, 1f);
            float horizontalXOffset = -(scale - 1f) * 0.75f;
            float horizontalYOffset = (scale - 1f) * 0.1f;
            float verticalXOffset = (scale - 1f) * 0.15f;
            float verticalYOffset = (scale - 1f) * 0.40f;


            string animationDirection = GetAxisDirection(direction);
            if (animationDirection == "Up")
            {
                itemAnimator.transform.localPosition = new Vector2(verticalXOffset, -verticalYOffset);
            } else if (animationDirection == "Down")
            {                
                itemAnimator.transform.localPosition = new Vector2(-verticalXOffset, verticalYOffset);
            } else
            {
                itemAnimator.transform.localPosition = new Vector2(horizontalXOffset, horizontalYOffset);
            }

            PlayAnimation(direction, "meleeAttack");
     

            Debug.Log("Melee Attack");
        }
    }

    public void MagicWeapon()
    {
       if (Input.GetMouseButtonDown(0))
        {    
            Vector2 direction = player.WorldPointPosition - (Vector2)player.transform.position;
            string animationDirection = GetAxisDirection(direction);
            player.Animator.SetTrigger("magicAttack" + animationDirection);
        }
    }




    public string GetAxisDirection(Vector2 direction)
    {
        direction = direction.normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return "LR";
        }
        else
        {
            if (direction.y > 0)
            {
                return "Up";
            }
            else
            {
                return "Down";
            }
        }
    }

    public void PlayAnimation(Vector2 direction, string type)
    {
        itemSpriteHolder.SetActive(true);
        direction = direction.normalized;
        string animationDirection = GetAxisDirection(direction);
        itemAnimator.UseAnimation("use" + animationDirection);
        player.Animator.SetTrigger("meleeAttack" + animationDirection);

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x < 0)
            {
                player.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else
            {
                player.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        else
        {
            player.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        player.Animator.SetTrigger(type + animationDirection);
    }
    


    public void FinishAttack()
    {
        itemSpriteHolder.SetActive(false);
        player.IsAttacking = false;
        itemAnimator.SetSpeed(1f);
        player.Animator.speed = 1f;
    }

    public void UsePotion(PotionData potionData)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (potionData == null)
            {
                return;
            }

            Debug.Log("Using potion with " + potionData.potionEffects.Length + " effects.");
            
            foreach (PotionEntry entry in potionData.potionEffects)
            {
                Debug.Log("Applying effect: " + entry.effect + " with magnitude: " + entry.magnitude + " for duration: " + entry.duration);
                PotionEffect effect = entry.effect;
                switch (effect)
                {
                    case PotionEffect.Heal:

                        player.Heal(entry.magnitude);
                        break;


                    
                }
            
            }

            // remove item
            inventory.SetItemAtIndex(inventory.CurrentItemIndex, null);
        }
    }

}


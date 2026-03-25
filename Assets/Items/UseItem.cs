using UnityEngine;

public class UseItem : MonoBehaviour
{
    private Player player;
    private Inventory inventory;
    private ItemData equippedItemData;
    private ItemAnimator itemAnimator;
    [SerializeField] private GameObject itemSpriteHolder;
    
    void Start()
    {
        player = FindFirstObjectByType<Player>();
        inventory = player.Inventory;
        itemAnimator = GetComponentInChildren<ItemAnimator>();

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
                MeleeData meleeData = equippedItemData as MeleeData;
                MeleeWeapon(meleeData.MeleeScale, meleeData.MeleeSpeed);
            } else if (equippedItemData is MagicData)
            {
                MagicWeapon();
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

}


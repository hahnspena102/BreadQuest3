using UnityEngine;
using System.Collections;

public class UseItem : MonoBehaviour
{
    private Player player;
    private Inventory inventory;
    private Item equippedItem;
    private ItemAnimator itemAnimator;
    [SerializeField] private GameObject itemSpriteHolder;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float healCooldownDuration = 30f;
    [SerializeField] private float glucoseCooldownDuration = 10f;

    public float HealCooldownDuration => healCooldownDuration;
    public float GlucoseCooldownDuration => glucoseCooldownDuration;
    
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
            } else if (equippedItemData is MagicData magicData)
            {
                MagicWeapon(magicData.ProjectileData, magicData);
            } else if (equippedItemData is RangedData rangedData)
            {
                RangedWeapon(rangedData);
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

            if (player.IsAttacking || player.IsInUIScreen) return;
            if (direction == Vector2.zero) return;
            

            player.IsAttacking = true;

            // ANIMATION
            itemAnimator.SetSpeed(speed);
            player.Animator.speed = speed;
            itemAnimator.transform.localScale = new Vector3(scale, scale, 1f);
            itemAnimator.transform.localRotation = Quaternion.identity;
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

             if (equippedItem.ItemData.IsConsumed)
            {
                DecrementItemCount();
            }
     


        }
    }

    public void MagicWeapon(ProjectileData projectileData, MagicData magicData)
    {
        
       if (Input.GetMouseButtonDown(0))
        {    
            if (player.IsAttacking || player.IsInUIScreen) return;
            if (magicData.GlucoseCost > player.PlayerData.CurrentGlucose) return;

            player.IsAttacking = true;
            player.PlayerData.CurrentGlucose -= magicData.GlucoseCost;
   

            SoundManager.instance.PlaySoundFXClip(magicData.GetCastSound(), player.transform);
  

            Vector2 direction = player.WorldPointPosition - (Vector2)player.transform.position;
            string animationDirection = GetAxisDirection(direction);
            player.Animator.SetTrigger("magicAttack" + animationDirection);
            if (direction == Vector2.zero) return;

            StartCoroutine(MagicAttackCoroutine(magicData, direction, magicData.ProjectileDelay));

            
        }

        if (equippedItem.ItemData.IsConsumed)
        {
            DecrementItemCount();
        }
    }

    IEnumerator MagicAttackCoroutine(MagicData magicData, Vector2 direction, float projectileDelay)
    {
        for (int i = 0; i < magicData.ProjectileCount; i++)
        {
            float angleOffset = (i - (magicData.ProjectileCount - 1) / 2f) * magicData.SpreadAngle;
            Vector2 rotatedDirection = Quaternion.Euler(0, 0, angleOffset) * direction.normalized;

            GameObject projectileObj = Instantiate(projectilePrefab, player.transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.InitializePlayerProjectile(rotatedDirection, magicData.ProjectileData, player);

            yield return new WaitForSeconds(projectileDelay);
        }

        yield return new WaitForSeconds(magicData.CooldownDuration); 

        player.IsAttacking = false;

    }

    private float timeCharging = 0f;
    public void RangedWeapon(RangedData rangedData)
    {
        if (player.IsInUIScreen) return;

        Vector2 direction = player.WorldPointPosition - (Vector2)player.transform.position;
        
        // Instant cast for zero charge time
        if (rangedData.ChargeTime <= 0f)
        {
            if (Input.GetMouseButtonDown(0)) {

            
                if (direction == Vector2.zero) return;
                
                string animationDirection = GetAxisDirection(direction);
                //player.Animator.SetTrigger("rangedAttack" + animationDirection);
                
                // Handle horizontal flip
                if (direction.x < 0)
                {
                    player.transform.localScale = new Vector3(-1f, 1f, 1f);
                }
                else
                {
                    player.transform.localScale = new Vector3(1f, 1f, 1f);
                }

                for (int i = 0; i < rangedData.ProjectileCount; i++)
                {
                    float angleOffset = (i - (rangedData.ProjectileCount - 1) / 2f) * rangedData.SpreadAngle;
                    Vector2 rotatedDirection = Quaternion.Euler(0, 0, angleOffset) * direction.normalized;

                    GameObject projectileObj = Instantiate(projectilePrefab, player.transform.position, Quaternion.identity);
                    Projectile projectile = projectileObj.GetComponent<Projectile>();
                    projectile.InitializePlayerProjectile(rotatedDirection, rangedData.ProjectileData, player);
                }

                //check if consumable
                if (equippedItem.ItemData.IsConsumed)
                {
                    DecrementItemCount();
                }
            }
            return;
        }
        
        bool isMouseHeld = Input.GetMouseButton(0);
        bool isMouseReleased = Input.GetMouseButtonUp(0);
        
        if (isMouseHeld)
        {    
            
            player.IsCharging = true;
            if (itemAnimator != null)
            {
                itemAnimator.transform.localScale = new Vector3(1f, 1f, 1f);
                itemAnimator.transform.localPosition = Vector2.zero;
                itemAnimator.BeginRangedCharge(rangedData);
            }
            // rotate sprite based on direction
            if (direction.x < 0)
            {
                player.transform.localScale = new Vector3(-1f, 1f, 1f);
              //  itemAnimator.transform.localScale = new Vector3(-1f, -1f, 1f);
                
             
            }
            else if (direction.x > 0)
            {
                player.transform.localScale = new Vector3(1f, 1f, 1f);
                itemAnimator.transform.localScale = new Vector3(1f, 1f, 1f);
               
            }
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle > 90f || angle < -90f)
            {
                angle = 180f - angle;
            }
            itemAnimator.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            

            player.Animator.SetBool("rangedAttack", true);
            itemAnimator.Animator.SetBool("isCharging", true);
            timeCharging += Time.deltaTime;
            if (timeCharging < rangedData.ChargeTime)
            {
                return;
            }
            
        }
        else if (isMouseReleased)
        {
            player.IsCharging = false;
            player.Animator.SetBool("rangedAttack", false);
            itemAnimator.Animator.SetBool("isCharging", false);
            if (itemAnimator != null)
            {
                itemAnimator.EndRangedCharge();
            }
            if (timeCharging >= rangedData.ChargeTime)
            {
                timeCharging = 0f;
            } else
            {
                return;
            }
            
           
            string animationDirection = GetAxisDirection(direction);
            if (direction == Vector2.zero) return;

            for (int i = 0; i < rangedData.ProjectileCount; i++)
            {
                float angleOffset = (i - (rangedData.ProjectileCount - 1) / 2f) * rangedData.SpreadAngle;
                Vector2 rotatedDirection = Quaternion.Euler(0, 0, angleOffset) * direction.normalized;

                GameObject projectileObj = Instantiate(projectilePrefab, player.transform.position, Quaternion.identity);
                Projectile projectile = projectileObj.GetComponent<Projectile>();
                projectile.InitializePlayerProjectile(rotatedDirection, rangedData.ProjectileData, player);
            }

            if (equippedItem.ItemData.IsConsumed)
            {
                DecrementItemCount();
            }

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
            if (player.IsAttacking || player.IsInUIScreen) return;
            if (potionData == null)
            {
                return;
            }

            Debug.Log("Using potion with " + potionData.potionEffects.Length + " effects.");
            bool appliedEffect = false;
            foreach (PotionEntry entry in potionData.potionEffects)
            {
                
                Debug.Log("Applying effect: " + entry.effect + " with magnitude: " + entry.magnitude + " for duration: " + entry.duration);
                PotionEffect effect = entry.effect;
                switch (effect)
                {
                    case PotionEffect.Heal:
                        if (player.PlayerData.HealCooldown > 0f)
                        {
                            Debug.Log("Heal effect is on cooldown for " + player.PlayerData.HealCooldown + " more seconds.");
                            break;
                        }
                        player.Heal(entry.magnitude);
                        player.PlayerData.HealCooldown = healCooldownDuration;
                        appliedEffect = true;
                        break;
                    case PotionEffect.Glucose:
                        if (player.PlayerData.GlucoseCooldown > 0f)
                        {
                            Debug.Log("Glucose effect is on cooldown for " + player.PlayerData.GlucoseCooldown + " more seconds.");
                            break;
                        }
                        player.PlayerData.CurrentGlucose = Mathf.Min(player.PlayerData.MaxGlucose, player.PlayerData.CurrentGlucose + entry.magnitude);
                        player.PlayerData.GlucoseCooldown = glucoseCooldownDuration;
                        appliedEffect = true;
                        break;


                    
                }
            
            }
            
            if (!appliedEffect)
            {
                Debug.Log("No effects were applied due to cooldowns. Potion will not be consumed.");
                return;
            }
            // remove item
            if (equippedItem.ItemData.IsConsumed)
            {
                DecrementItemCount();
            } 
        }
    }

    public void DecrementItemCount()
    {
        if (equippedItem == null)
        {
            return;
        }

        equippedItem.Count = Mathf.Max(0, equippedItem.Count - 1);
        if (equippedItem.Count == 0)
        {
            inventory.SetItemAtIndex(inventory.CurrentItemIndex, null);
        }
    }

}


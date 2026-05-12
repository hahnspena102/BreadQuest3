using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    [SerializeField]private Camera mainCamera;
    [SerializeField] private SpriteRenderer itemSpriteHolder;



    [Header("Player Data")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject hoverItem;
    private readonly List<GameObject> overlappingItems = new List<GameObject>();
    private bool isAttacking = false;
    private bool isCharging = false;
    private bool isDashing = false;
    private bool isMenuing = false;
    private float invulnerabilityTimer = 0f;
    
    private float invulnerabilityDuration = 1f;
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.1f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float dashDampenTime = 0.15f;
    [SerializeField] private float dashDampenStrength = 8f;
    private bool isDampening = false;
    private float dampenTimer = 0f;
    private Vector2 dampenVelocity;
    private int flashCount = 3;
    private string directionFacing = "Down";
    private int previousLevel;

    [Header("Input")]
    public InputActionReference moveAction;
    public InputActionReference dashAction;
    public InputActionReference numberKeyAction;
    public InputActionReference equipAction;
    public InputActionReference dropAction;
    public InputActionReference useAction;

    [Header("Managers")]
    [ReadOnly] public ItemManager itemManager;
    [ReadOnly] public GameManager gameManager;
    [ReadOnly] public PopupManager popupManager;

    [Header("Debug")]
    [SerializeField]private bool debugInvulnerability = false;

    private Vector2 _moveDirection;

    private float _mouseScrollY;
    private Vector2 worldPointPosition;
    private Vector2 dashDirection = Vector2.down;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;

    public Vector2 WorldPointPosition { get => worldPointPosition; set => worldPointPosition = value; }
    public Inventory Inventory { get => inventory; set => inventory = value; }
    public global::System.Boolean IsAttacking { get => isAttacking; set => isAttacking = value; }
    public global::System.String DirectionFacing { get => directionFacing; set => directionFacing = value; }
    public PlayerData PlayerData { get => playerData; set => playerData = value; }
    public Animator Animator { get => animator; set => animator = value; }
    public global::System.Boolean IsCharging { get => isCharging; set => isCharging = value; }
    public GameObject HoverItem { get => hoverItem; set => hoverItem = value; }
    public global::System.Single DashSpeed { get => dashSpeed; set => dashSpeed = value; }
    public global::System.Single DashDuration { get => dashDuration; set => dashDuration = value; }
    public global::System.Single DashCooldown { get => dashCooldown; set => dashCooldown = value; }
    public global::System.Single DashCooldownTimer { get => dashCooldownTimer; set => dashCooldownTimer = value; }
    public global::System.Boolean IsMenuing { get => isMenuing; set => isMenuing = value; }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        itemManager = FindFirstObjectByType<ItemManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        popupManager = FindFirstObjectByType<PopupManager>();
        
        inventory.EquippedItem = inventory.GetItemAtIndex(inventory.CurrentItemIndex);

        previousLevel = playerData.Level;
        StartCoroutine(StatCoroutine());
    }

    private IEnumerator StatCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            
            if (playerData.CurrentGlucose < playerData.MaxGlucose)
            {
                float glucoseRegenAmount = 1f + playerData.GlucoseRegenBonus;
                playerData.CurrentGlucose += glucoseRegenAmount;
                if (playerData.CurrentGlucose > playerData.MaxGlucose)
                {
                    playerData.CurrentGlucose = playerData.MaxGlucose;
                }
            }
            if (playerData.CurrentHealth < playerData.MaxHealth)
            {
                float healthRegenAmount = playerData.HealthRegenBonus;
                Heal(healthRegenAmount);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        playerData.UpdateLevel();
        
        playerData.MaxHealth = 100f + (playerData.Level - 1) * 20f;
        playerData.MaxGlucose = 50f + (playerData.Level - 1) * 10f;
        
        if (playerData.Level > previousLevel)
        {
            playerData.CurrentHealth = playerData.MaxHealth;
            playerData.CurrentGlucose = playerData.MaxGlucose;
            Debug.Log("Level Up! Current Level: " + playerData.Level);
            previousLevel = playerData.Level;
        }

        playerData.CalculateToppingBonuses();

        if (playerData.CurrentHealth <= 0 && debugInvulnerability == false)
        {
            gameManager.GameOver();
        }

        if (playerData.HealCooldown > 0f)
        {
            playerData.HealCooldown -= Time.deltaTime;
        }
        if (playerData.GlucoseCooldown > 0f)
        {
            playerData.GlucoseCooldown -= Time.deltaTime;
        }

        if (invulnerabilityTimer > 0f)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;

                // Start dampening from current velocity
                if (rb != null)
                {
                    dampenVelocity = rb.linearVelocity;
                    isDampening = true;
                    dampenTimer = dashDampenTime;
                }
            }
        }

        _moveDirection = moveAction.action.ReadValue<Vector2>();

        bool dashPressed = (dashAction != null && dashAction.action != null && dashAction.action.WasPressedThisFrame())
            || (Keyboard.current != null && (Keyboard.current.leftShiftKey.wasPressedThisFrame || Keyboard.current.rightShiftKey.wasPressedThisFrame));

        if (!isDashing && !isAttacking && dashPressed && dashCooldownTimer <= 0f && _moveDirection.sqrMagnitude > 0.001f)
        {
            dashDirection = _moveDirection.normalized;
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
        }

        if (rb)
        {
            
            if (!isAttacking && !isCharging && !isDashing && !isMenuing)
            {    
                if (_moveDirection.y != 0 && _moveDirection.x == 0) {
                    transform.localScale = new Vector3(1f, 1f, 1f);
                }
                
                if (_moveDirection.x < 0) {
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                    directionFacing = "Left";
                } else if (_moveDirection.x > 0) {
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    directionFacing = "Right";
                }

                if (_moveDirection.y > 0) {
                    directionFacing = "Up";
                } else if (_moveDirection.y < 0) {
                    directionFacing = "Down";
                }
            }
            
            
            if (!isMenuing)
            {
                animator.SetFloat("speed", _moveDirection.magnitude);
                animator.SetFloat("horizontalSpeed", Mathf.Abs(_moveDirection.x));
                animator.SetFloat("vertical", _moveDirection.y);
            }
        }


        //Debug.Log(numberKeyAction.action.ReadValue<float>());
       

        worldPointPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (!isAttacking && !isCharging && !isDashing)
        { 
            if (equipAction.action.WasPressedThisFrame())
            {
                if (hoverItem != null)
                {
                    EquipItem(hoverItem);
                }
            }

            if (dropAction.action.WasPressedThisFrame())
            {
                if (inventory.EquippedItem.ItemData != null)
                {
                    DropItem();
                    
                }
            }

            int _numberKey = Mathf.FloorToInt(numberKeyAction.action.ReadValue<float>());
            inventory.CycleTo(_numberKey - 1);
            ItemData itemData = inventory.EquippedItem != null ? inventory.EquippedItem.ItemData : null;
            itemSpriteHolder.sprite = itemData != null ? itemData.ItemSprite : null;
        }
        

    }
 

    void FixedUpdate()
    {
        if (rb)
        {
            if (isDashing)
            {
                float totalDashSpeed = dashSpeed * (isCharging ? 0.5f : 1f);
                rb.linearVelocity = dashDirection * totalDashSpeed;
                return;
            }

            if (isDampening)
            {
                dampenTimer -= Time.fixedDeltaTime;

                dampenVelocity = Vector2.Lerp(
                    dampenVelocity,
                    Vector2.zero,
                    dashDampenStrength * Time.fixedDeltaTime
                );

                rb.linearVelocity = dampenVelocity;

                if (dampenTimer <= 0f)
                {
                    isDampening = false;
                    rb.linearVelocity = Vector2.zero;
                }

                return;
            }

            float totalSpeed = playerData.Speed * (1f + playerData.SpeedBonus);

            if (isAttacking)
                totalSpeed *= 0.8f;

            if (isCharging)
                totalSpeed *= 0.25f;

            if (isMenuing)
            {
                _moveDirection = Vector2.zero;
            } 
            
            rb.linearVelocity = _moveDirection * totalSpeed;
            
          
            
            
            
        }
    }

    
    public void TakeDamage(float damage)
    {
        if (invulnerabilityTimer > 0f) return;

        playerData.CurrentHealth -= damage;
        animator.SetTrigger("hurt");
        itemSpriteHolder.sprite = null;
        invulnerabilityTimer = invulnerabilityDuration; 
        //  Damage Taken=Raw Damage×(ConstantConstant+Defense)Damage Taken equals Raw Damage cross open paren the fraction with numerator Constant and denominator Constant plus Defense end-fraction close parenDamage Taken=Raw Damage×ConstantConstant+Defense
        float totalDamage = damage * (100f / (100f + playerData.DefenseBonus));
        
        popupManager.ShowDamagePopup(transform.position, (int)totalDamage, false, true);

        StartCoroutine(DamageFlash());
        
    }

    public void Heal(float amount)
    {
        playerData.CurrentHealth += amount;
        if (playerData.CurrentHealth > playerData.MaxHealth)
        {
            playerData.CurrentHealth = playerData.MaxHealth;
        }
    }

    private IEnumerator DamageFlash()
    {
        float pulseDuration = 0.1f;
        sr.color = Color.red;
        yield return new WaitForSeconds(pulseDuration);

        sr.color = Color.white;

        float flashInterval = invulnerabilityDuration / (flashCount * 2);
        for (int i = 0; i < flashCount; i++)
        {
            sr.color = new Color(1.000f, 1.000f, 1.000f, 0.5f);
            yield return new WaitForSeconds(flashInterval);
            sr.color = Color.white;
            yield return new WaitForSeconds(flashInterval);
        }

        sr.color = Color.white; 

        isAttacking = false;
    }

    public void EquipItem(GameObject itemObj)
    {
        DroppedItem droppedItem = itemObj.GetComponent<DroppedItem>();
        if (droppedItem == null || droppedItem.Item == null || droppedItem.Item.ItemData == null)
        {
            return;
        }

        int startingDroppedCount = Mathf.Max(1, droppedItem.Item.Count);
        bool pickedUpFully = inventory.AddItem(droppedItem);
        if (pickedUpFully)
        {
            Destroy(itemObj);
        }
        else
        {
            bool noEmptySlots = inventory.NextEmptySlot() == -1;
            bool nothingWasAdded = droppedItem.Item.Count == startingDroppedCount;

            if (noEmptySlots && nothingWasAdded)
            {
                Item equippedBeforeSwap = inventory.GetItemAtIndex(inventory.CurrentItemIndex);
                Item incomingItem = ItemFactory.Clone(droppedItem.Item);

                if (incomingItem != null)
                {
                    inventory.SetItemAtIndex(inventory.CurrentItemIndex, incomingItem);
                    inventory.EquippedItem = incomingItem;

                    if (equippedBeforeSwap != null)
                    {
                        droppedItem.Item = ItemFactory.Clone(equippedBeforeSwap);
                    }
                    else
                    {
                        Destroy(itemObj);
                    }
                }
            }
        }

        Item equipped = inventory.GetItemAtIndex(inventory.CurrentItemIndex);
        inventory.EquippedItem = equipped;
        itemSpriteHolder.sprite = equipped != null && equipped.ItemData != null ? equipped.ItemData.ItemSprite : null;
        RefreshHoverItem();
        
    }
   
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            GameObject item = other.gameObject;
            if (!overlappingItems.Contains(item))
            {
                overlappingItems.Add(item);
            }
            RefreshHoverItem();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            overlappingItems.Remove(other.gameObject);
            RefreshHoverItem();
        }
    }

    private void RefreshHoverItem()
    {
        overlappingItems.RemoveAll(item => item == null || !item.CompareTag("Item"));
        if (overlappingItems.Count == 0)
        {
            hoverItem = null;
            return;
        }

        GameObject closestItem = null;
        float closestDistanceSqr = float.MaxValue;
        Vector3 playerPosition = transform.position;

        for (int i = 0; i < overlappingItems.Count; i++)
        {
            GameObject item = overlappingItems[i];
            float distanceSqr = (item.transform.position - playerPosition).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestItem = item;
            }
        }

        hoverItem = closestItem;
    }

    
    public void DropItem()
    {
        Vector2 dropPosition = (Vector2)transform.position + new Vector2(0f, -0.5f);
        itemManager.SpawnItem(inventory.EquippedItem, dropPosition);
        inventory.SetItemAtIndex(inventory.CurrentItemIndex, null);
        inventory.EquippedItem = null;
        //inventory.CurrentItemIndex = -1;
        itemSpriteHolder.sprite = null;
    }

}
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections;

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
    private bool isAttacking = false;
    private float invulnerabilityTimer = 0f;
    
    private float invulnerabilityDuration = 1f;
    private int flashCount = 3;
    private string directionFacing = "Down";

    [Header("Input")]
    public InputActionReference moveAction;
    public InputActionReference numberKeyAction;
    public InputActionReference equipAction;
    public InputActionReference dropAction;
    public InputActionReference useAction;

    [Header("Managers")]
    [ReadOnly] public ItemManager itemManager;
    [ReadOnly] public GameManager gameManager;

    [Header("Debug")]
    [SerializeField]private bool debugInvulnerability = false;

    private Vector2 _moveDirection;

    private float _mouseScrollY;
    private Vector2 worldPointPosition;

    public Vector2 WorldPointPosition { get => worldPointPosition; set => worldPointPosition = value; }
    public Inventory Inventory { get => inventory; set => inventory = value; }
    public global::System.Boolean IsAttacking { get => isAttacking; set => isAttacking = value; }
    public global::System.String DirectionFacing { get => directionFacing; set => directionFacing = value; }
    public PlayerData PlayerData { get => playerData; set => playerData = value; }
    public Animator Animator { get => animator; set => animator = value; }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        itemManager = FindFirstObjectByType<ItemManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        inventory.CurrentItemIndex = 0;
        inventory.EquippedItemData = inventory.GetItemAtIndex(inventory.CurrentItemIndex);


        StartCoroutine(StatCoroutine());
    }

    private IEnumerator StatCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (playerData.CurrentGlucose < playerData.MaxGlucose)
            {
                playerData.CurrentGlucose += 1f;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        playerData.UpdateLevel();
        if (playerData.CurrentHealth <= 0 && debugInvulnerability == false)
        {
            gameManager.GameOver();
        }

        if (invulnerabilityTimer > 0f)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }
        _moveDirection = moveAction.action.ReadValue<Vector2>();

        if (rb)
        {
            if (_moveDirection.y != 0 && _moveDirection.x == 0) {
                    transform.localScale = new Vector3(1f, 1f, 1f);
            }
            if (!isAttacking)
            {    
                
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
            
            

            animator.SetFloat("speed", _moveDirection.magnitude);
            animator.SetFloat("horizontalSpeed", Mathf.Abs(_moveDirection.x));
            animator.SetFloat("vertical", _moveDirection.y);
        }


        //Debug.Log(numberKeyAction.action.ReadValue<float>());
       

        worldPointPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (!isAttacking)
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
                if (inventory.EquippedItemData != null)
                {
                    DropItem();
                    
                }
            }

            int _numberKey = Mathf.FloorToInt(numberKeyAction.action.ReadValue<float>());
            inventory.CycleTo(_numberKey - 1);
            itemSpriteHolder.sprite = inventory.EquippedItemData != null ? inventory.EquippedItemData.ItemSprite : null;
        }
        

    }
 

    void FixedUpdate()
    {
        if (rb)
        {
            float totalSpeed = playerData.Speed;
            if (isAttacking)
            {
                totalSpeed *= 0.8f;
            }
            rb.linearVelocity = _moveDirection * totalSpeed;
        }
        
        
    }

    
    public void TakeDamage(float damage)
    {
        if (invulnerabilityTimer > 0f) return;

        playerData.CurrentHealth -= damage;
        animator.SetTrigger("hurt");
        invulnerabilityTimer = invulnerabilityDuration; 

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
        Item item = itemObj.GetComponent<Item>();
        if (item != null && item.ItemData != null)
        {
            if (inventory.GetItemAtIndex(inventory.CurrentItemIndex) == null)
            { 
                inventory.SetItemAtIndex(inventory.CurrentItemIndex, item.ItemData);
            } else
            {
                int nextEmptySlot = inventory.NextEmptySlot();
                if (nextEmptySlot == -1)
                {
                    DropItem();
                    inventory.SetItemAtIndex(inventory.CurrentItemIndex, item.ItemData);
                } else
                {
                    inventory.SetItemAtIndex(nextEmptySlot, item.ItemData);
                }
            
                
            }
            inventory.EquippedItemData = item.ItemData;
            itemSpriteHolder.sprite = item.ItemData.ItemSprite;
            Destroy(itemObj);
        }
    }
   
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            hoverItem = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            hoverItem = null;
        }
    }

    
    public void DropItem()
    {
        Vector2 dropPosition = (Vector2)transform.position + new Vector2(0f, -0.5f);
        itemManager.SpawnItem(inventory.EquippedItemData, dropPosition);
        inventory.SetItemAtIndex(inventory.CurrentItemIndex, null);
        inventory.EquippedItemData = null;
        //inventory.CurrentItemIndex = -1;
        itemSpriteHolder.sprite = null;
    }

}

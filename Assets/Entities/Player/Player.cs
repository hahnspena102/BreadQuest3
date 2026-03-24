using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField]private Camera mainCamera;
    [SerializeField] private SpriteRenderer itemSpriteHolder;
    [SerializeField] private GameObject attackBox;



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




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        itemManager = FindFirstObjectByType<ItemManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        inventory.CurrentItemIndex = 0;
        inventory.EquippedItemData = inventory.GetItemAtIndex(inventory.CurrentItemIndex);

        attackBox.SetActive(false);

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
            
            

            anim.SetFloat("speed", _moveDirection.magnitude);
            anim.SetFloat("horizontalSpeed", Mathf.Abs(_moveDirection.x));
            anim.SetFloat("vertical", _moveDirection.y);
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

    public void MeleeAttack(Vector2 direction)
    {    
        if (isAttacking) return;
        if (direction == Vector2.zero) return;
        AttackAnimation(direction, "melee");
        isAttacking = true;
        attackBox.SetActive(true);
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
             attackBox.transform.localPosition = new Vector2(1f, 0f);
        }
        else
        {
            attackBox.transform.localPosition = new Vector2(0f, direction.y > 0 ? 1f : -1f);
        }
       
    }

    public void MagicAttack(Vector2 direction)
    {
        if (isAttacking) return;
        if (direction == Vector2.zero) return;
        AttackAnimation(direction, "magic");
        isAttacking = true;
    }

    public void AttackAnimation(Vector2 direction, string type)
    {
        
        itemSpriteHolder.gameObject.SetActive(true);
        itemSpriteHolder.sortingOrder = 1;
        direction = direction.normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                ////Debug.Log("Left Attack");
            }
            else
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                ////Debug.Log("Right Attack");
            }

            anim.SetTrigger(type + "AttackLR");
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            if (direction.y > 0)
            {
                anim.SetTrigger(type + "AttackUp");
                //itemSpriteHolder.sortingOrder = -1;
                //Debug.Log("Up Attack");
            }
            else
            {
                anim.SetTrigger(type + "AttackDown");
                //Debug.Log("Down Attack");
            }
        }
    }


    public void FinishAttack()
    {
        itemSpriteHolder.gameObject.SetActive(false);
        isAttacking = false;
        attackBox.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        if (invulnerabilityTimer > 0f) return;

        playerData.CurrentHealth -= damage;
        anim.SetTrigger("hurt");
        invulnerabilityTimer = invulnerabilityDuration; 

        StartCoroutine(DamageFlash());
        
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

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class Player : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField]private Camera mainCamera;
    [SerializeField] private SpriteRenderer itemSpriteHolder;



    [Header("Player Data")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Inventory inventory;
    private bool isAttacking = false;

    [Header("Input")]
    public InputActionReference moveAction;
    public InputActionReference numberKeyAction;


    private Vector2 _moveDirection;

    private float _mouseScrollY;
    private Vector2 worldPointPosition;

    public Vector2 WorldPointPosition { get => worldPointPosition; set => worldPointPosition = value; }
    public Inventory Inventory { get => inventory; set => inventory = value; }
    public global::System.Boolean IsAttacking { get => isAttacking; set => isAttacking = value; }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        inventory.CurrentItemIndex = 0;


    }

    // Update is called once per frame
    void Update()
    {
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
                } else if (_moveDirection.x > 0) {
                    transform.localScale = new Vector3(1f, 1f, 1f);
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
            int _numberKey = Mathf.FloorToInt(numberKeyAction.action.ReadValue<float>());
            inventory.CycleTo(_numberKey - 1);
            itemSpriteHolder.sprite = inventory.EquippedItemData != null ? inventory.EquippedItemData.ItemSprite : null;
        }
        

    }
 

    void FixedUpdate()
    {
        if (rb)
        {
            rb.linearVelocity = _moveDirection * playerData.Speed;
        }
        
        
    }

  public void Attack(Vector2 direction, string type)
{
    if (isAttacking) return;
    if (direction == Vector2.zero) return;

    isAttacking = true;
    itemSpriteHolder.gameObject.SetActive(true);
    itemSpriteHolder.sortingOrder = 1;
    direction = direction.normalized;

    if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
    {
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            Debug.Log("Left Attack");
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            Debug.Log("Right Attack");
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
            Debug.Log("Up Attack");
        }
        else
        {
            anim.SetTrigger(type + "AttackDown");
            Debug.Log("Down Attack");
        }
    }
}


    public void FinishAttack()
    {
        itemSpriteHolder.gameObject.SetActive(false);
        isAttacking = false;
    }
}

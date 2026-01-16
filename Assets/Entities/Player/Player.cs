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



    [Header("Player Data")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Inventory inventory;

    [Header("Input")]
    public InputActionReference moveAction;
    public InputActionReference numberKeyAction;


    private Vector2 _moveDirection;

    private float _mouseScrollY;
    private Vector2 worldPointPosition;

    public Vector2 WorldPointPosition { get => worldPointPosition; set => worldPointPosition = value; }
    public Inventory Inventory { get => inventory; set => inventory = value; }



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
            if (_moveDirection.x < 0)
            {
                sr.flipX = true;
            }
            else if (_moveDirection.x > 0)
            {
                sr.flipX = false;
            }
            anim.SetFloat("speed", _moveDirection.magnitude);
        }


        //Debug.Log(numberKeyAction.action.ReadValue<float>());
        int _numberKey = Mathf.FloorToInt(numberKeyAction.action.ReadValue<float>());

        worldPointPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

    
        inventory.CycleTo(_numberKey - 1);

    }
 

    void FixedUpdate()
    {
        if (rb)
        {
            rb.linearVelocity = _moveDirection * playerData.Speed;
        }
        
        
    }
}

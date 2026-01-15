using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class Player : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;



    [Header("Player Data")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Inventory inventory;

    [Header("Input")]
    public InputActionReference moveAction;
    public InputActionReference numberKeyAction;


    private Vector2 _moveDirection;
    private Vector2 _pointPosition;
    private float _mouseScrollY;
    
    

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

    
        inventory.CycleTo(_numberKey - 1);

        _pointPosition = Input.mousePosition;
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click at: " + _pointPosition);
        }
        
    }
 

    void FixedUpdate()
    {
        if (rb)
        {
            rb.linearVelocity = _moveDirection * playerData.Speed;
        }
        
        
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    public InputActionReference moveAction;

    [Header("Player Data")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Inventory inventory;

    [Header("Player Data")]
    private Vector2 _moveDirection;
    private Vector2 _mouseScrollY;

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

        _mouseScrollY = Mouse.current.scroll.ReadValue();
        float _scrollDirection = _mouseScrollY.y;
        
        if (_scrollDirection > 0)
        {
            inventory.CycleItem(-1);
        } else if (_scrollDirection < 0)
        {
            inventory.CycleItem(1);
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

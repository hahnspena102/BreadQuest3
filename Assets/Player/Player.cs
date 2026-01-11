using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Basics")]
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    public InputActionReference moveAction;

    [Header("Basics")]
    public float speed;
    private Vector2 _moveDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
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
    }

    void FixedUpdate()
    {
        if (rb)
        {
            rb.linearVelocity = _moveDirection * speed;
        }
        
        
    }
}

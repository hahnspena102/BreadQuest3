using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public InputActionReference moveAction;
    private Vector2 _moveDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _moveDirection = moveAction.action.ReadValue<Vector2>();
        
        Debug.Log(_moveDirection);
    }

    void FixedUpdate()
    {
        
        rb.linearVelocity = new Vector2(_moveDirection.x * speed, _moveDirection.y * speed);
        
    }
}

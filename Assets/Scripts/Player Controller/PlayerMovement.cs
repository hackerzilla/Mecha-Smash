using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float airControl = 0.6f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private int maxJumps = 1;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRadius = 0.1f;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private int jumpsRemaining;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        GroundCheck();
        Move();
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        if (isGrounded)
            jumpsRemaining = maxJumps;
    }

    public void Move()
    {
        float control = isGrounded ? 1f : airControl;
        float targetVelocityX = moveInput.x * moveSpeed * control;

        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void Jump()
    {
        if (jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpsRemaining -= 1;
        }
    }
}
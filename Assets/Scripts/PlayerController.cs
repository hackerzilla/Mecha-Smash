using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float airControl = 0.6f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private int maxJumps = 1;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.1f;

    [Header("User Interface")]
    public PlayerUI playerUI;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private int jumpsRemaining;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Random Color
        GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);
    }

    private void FixedUpdate()
    {
        // Ground Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        if (isGrounded)
            jumpsRemaining = maxJumps;

        // Movement
        float control = isGrounded ? 1f : airControl;
        float targetVelocityX = moveInput.x * moveSpeed * control;
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("Move pressed via: " + context.control.device.displayName);
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Jump pressed via: " + context.control.device.displayName);
            Jump();
        }
    }

    public void Submit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerUI.SetReady();
            playerUI.CheckReady();
            Debug.Log($"{gameObject.name} Submit pressed by {GetComponent<PlayerInput>().devices[0].displayName}");
        }
    }

    private void Jump()
    {
        // Check if we have enough jumps
        if (jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpsRemaining -= 1;
        }
    }
}

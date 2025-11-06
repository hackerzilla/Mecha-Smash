using System.Collections;
using UnityEngine;

public class MechMovement : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float airControl = 0.6f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private int maxJumps = 1;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private int jumpsRemaining;
    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isMovementOverridden = false;
    private ContactFilter2D groundContactFilter;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Set up contact filter for ground detection
        groundContactFilter = new ContactFilter2D();
        groundContactFilter.useLayerMask = true;
        groundContactFilter.layerMask = groundLayer;
    }

    void FixedUpdate()
    {
        if (isMovementOverridden)
        {
            return;
        }

        // Ground Check using Rigidbody2D collision detection
        isGrounded = rb.IsTouching(groundContactFilter);
        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
        }

        // Update facing direction
        if (moveInput.x > 0.1f)
        {
            isFacingRight = true;
        }
        else if (moveInput.x < -0.1f)
        {
            isFacingRight = false;
        }

        // Apply movement
        float targetVelocityX;
        if (isGrounded)
        {
            targetVelocityX = moveInput.x * moveSpeed;
        }
        else
        {
            targetVelocityX = Mathf.Lerp(rb.linearVelocity.x, moveInput.x * moveSpeed, Time.fixedDeltaTime * airControl);
        }
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void Jump()
    {
        // Check if we have enough jumps
        if (jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpsRemaining -= 1;
        }
    }

    public void SetMovementOverride(bool isOverriding)
    {
        isMovementOverridden = isOverriding;
    }

    public void SetMovementOverride(bool isOverriding, float duration)
    {
        isMovementOverridden = isOverriding;
        if (isOverriding && duration > 0)
        {
            StartCoroutine(ResetMovementOverride(duration));
        }
    }

    private IEnumerator ResetMovementOverride(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        isMovementOverridden = false;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetMaxJumps(int jumps)
    {
        maxJumps = jumps;
        jumpsRemaining = jumps;
    }

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public float GetFacingDirection()
    {
        return isFacingRight ? 1f : -1f;
    }
}

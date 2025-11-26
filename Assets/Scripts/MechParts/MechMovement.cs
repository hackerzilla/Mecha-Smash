using System.Collections;
using UnityEngine;

public class MechMovement : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] public float moveSpeed = 8f;
    [SerializeField] private float airControl = 0.6f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private int maxJumps = 1;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Death Zone")]
    [SerializeField] private float deathY = -25f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private int jumpsRemaining;
    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isMovementOverridden = false;
    private ContactFilter2D groundContactFilter;
    private Animator skeletonAnimator;
    private const float MOVING_THRESHOLD = 1.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Set up contact filter for ground detection
        groundContactFilter = new ContactFilter2D();
        groundContactFilter.useLayerMask = true;
        groundContactFilter.layerMask = groundLayer;

        // Get skeleton animator reference
        MechController mechController = GetComponent<MechController>();
        if (mechController != null && mechController.skeletonRig != null)
        {
            skeletonAnimator = mechController.skeletonRig.GetComponent<Animator>();
        }
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

        // Update animator parameters
        UpdateAnimatorParameters();

        // Update facing direction
        if (moveInput.x > 0.1f)
        {
            isFacingRight = true;
        }
        else if (moveInput.x < -0.1f)
        {
            isFacingRight = false;
        }

        // Update sprite direction to match facing
        UpdateSpriteDirection();

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

        // Check if mech has fallen off the map
        if (transform.position.y < deathY)
        {
            MechHealth mechHealth = GetComponent<MechHealth>();
            if (mechHealth != null && mechHealth.currentHealth > 0)
            {
                mechHealth.Die();
            }
        }
    }

    private void UpdateSpriteDirection()
    {
        // Flip the sprite horizontally based on facing direction
        float scaleX = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(
            isFacingRight ? scaleX : -scaleX,
            transform.localScale.y,
            transform.localScale.z
        );
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    /// <summary>
    /// Checks if jump is available and decrements jump counter.
    /// Returns true if jump can proceed (animation should play).
    /// </summary>
    public bool TryInitiateJump()
    {
        if (jumpsRemaining > 0)
        {
            jumpsRemaining -= 1;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Applies the actual jump physics force. Called by Animation Event via MechController.
    /// </summary>
    public void ApplyJumpForce()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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

    private void UpdateAnimatorParameters()
    {
        if (skeletonAnimator == null) return;

        // isMoving: true when horizontal speed exceeds threshold
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > MOVING_THRESHOLD;
        skeletonAnimator.SetBool("isMoving", isMoving);

        // isOnGround: directly uses existing ground check
        skeletonAnimator.SetBool("isOnGround", isGrounded);
    }
}

using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Mech Configuration")]

    public MechController mechPrefab;
    public HeadPart headPrefab;
    public TorsoPart torsoPrefab;
    public ArmsPart armsPrefab;
    public LegsPart legsPrefab;
    
    [Header("Runtime")]
    public MechController mechInstance;
    
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
    public PlayerCard playerCard;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private int jumpsRemaining;
    private bool isGrounded;
    private PlayerInput playerInput;
    private bool isFacingRight = true;
    private bool isAbilityOverridingMovement = false;
    
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        if (mechInstance == null)
        {
            Assert.NotNull(mechPrefab);
            mechInstance = Instantiate(mechPrefab, this.transform);
            mechInstance.AssembleMechFromPrefabParts(torsoPrefab, headPrefab, armsPrefab, legsPrefab);
        }
        Assert.NotNull(mechInstance);

        rb = mechInstance.GetComponent<Rigidbody2D>();

        if (groundCheck == null)
        {
            groundCheck = mechInstance.transform.Find("GroundCheck");
            Assert.NotNull(groundCheck, "need 'GroundCheck' object in Mech prefab");
        }

        ApplyLegStats();

        DisableInputMapping("Player");
        EnableInputMapping("UI");
    }

    public Rigidbody2D GetRigidbody() { return rb; }
    public bool IsGrounded() { return isGrounded; }
    public float GetFacingDirection() { return isFacingRight ? 1f : -1f; }


    private void FixedUpdate()
    {
        if (isAbilityOverridingMovement)
        {
            return;
        }

        // Ground Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
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

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Jump();
        }
    }

    public void OnHeadAbility(InputAction.CallbackContext context)
    {
        if (mechInstance != null)
        {
            mechInstance.SpecialAttack(this, context);
        }
    }

    public void OnTorsoAbility(InputAction.CallbackContext context)
    {
        if (mechInstance != null)
        {
            mechInstance.DefensiveAbility(this, context);
        }
    } 
    
    public void OnArmsAbility(InputAction.CallbackContext context)
    {
        if (mechInstance != null)
        {
            mechInstance.BasicAttack(this, context);
        }
    }

    public void OnLegsAbility(InputAction.CallbackContext context)
    {
        if (mechInstance != null)
        {
            mechInstance.MovementAbility(this, context);
        }
    }
    
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerCard.OnNavigate(context);
        }
    }
    
    public void SetMovementOverride(bool isOverriding)
    {
        isAbilityOverridingMovement = isOverriding;
    }

    public void SetMovementOverride(bool isOverriding, float duration)
    {
        isAbilityOverridingMovement = isOverriding;
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
        
        isAbilityOverridingMovement = false;
    }

    public void Submit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerCard.OnSubmit(context);
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
            mechInstance.Jump();
        }
    }

    private void ApplyLegStats()
    {
        if (mechInstance.legsInstance != null)
        {
            this.moveSpeed = mechInstance.legsInstance.moveSpeed;
            this.maxJumps = mechInstance.legsInstance.maxJumps;
            this.jumpsRemaining = this.maxJumps;
            // Debug.Log($"Legs Stats Applied: {mechInstance.legsInstance.name} (Speed: {moveSpeed}, Jumps: {maxJumps})");
        }
        else
        {
            Debug.LogError("Mech legsInstance가 null이라 스탯을 적용할 수 없습니다.");
            this.moveSpeed = 5f;
            this.maxJumps = 1;
            this.jumpsRemaining = 1;
        }
    }
    
    public void SwapMechPart(MechPart newPartPrefab)
    {
        if (mechInstance != null)
        {
            mechInstance.SwapPart(newPartPrefab);

            if (newPartPrefab is LegsPart)
            {
                ApplyLegStats();
            }
        }
        else
        {
            Debug.LogWarning("For some reason the mechInstance was null (from SwapMechPart in PlayerController on "+ name + ")");
        }
    }
    
    public void OnGameStart()
    {
        DisableInputMapping("UI");
        EnableInputMapping("Player");
    }

    public void DisableInputMapping(string mapName)
    {
        var map = playerInput.actions.FindActionMap(mapName); 
        Debug.Assert(map != null, "Could not find map: " + mapName);
        map.Disable();
    }
    public void EnableInputMapping(string mapName)
    {
        var map = playerInput.actions.FindActionMap(mapName); 
        Debug.Assert(map != null, "Could not find map: " + mapName);
        map.Enable();
    }
}

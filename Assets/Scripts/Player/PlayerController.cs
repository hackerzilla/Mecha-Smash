using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
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

    [Header("Visual Identification")]
    [SerializeField] private Color outlineColor = Color.white;

    [Header("User Interface")]
    public PlayerCard playerCard;
    [SerializeField] private float submitCooldownAfterJoin = 1f;

    [Header("Events")]
    public UnityEvent onPlayerDeath = new UnityEvent();

    private PlayerInput playerInput;
    private float canSubmitAfterTime = 0f;
    public bool canMove;

    public int playerNumber { get; private set; }

    void Awake()
    {
        if (mechInstance == null)
        {
            Assert.NotNull(mechPrefab);
            mechInstance = Instantiate(mechPrefab, this.transform);
        }
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerNumber = playerInput.playerIndex + 1; // Convert 0-based index to 1-based player number

        Assert.NotNull(mechInstance);

        // Set player index for outline layer isolation (must be before mech Start runs)
        MechOutlineRenderer outlineRenderer = mechInstance.GetComponent<MechOutlineRenderer>();
        if (outlineRenderer != null)
        {
            outlineRenderer.SetPlayerIndex(playerInput.playerIndex);
        }

        // Apply outline color to mech
        mechInstance.SetOutlineColor(outlineColor);

        // Subscribe to mech's death event
        MechHealth mechHealth = mechInstance.GetComponent<MechHealth>();
        if (mechHealth != null)
        {
            mechHealth.onDeath.AddListener(OnMechDeath);
        }
        else
        {
            Debug.LogError($"MechHealth component not found on {mechInstance.name}");
        }

        DisableInputMapping("Player");
        EnableInputMapping("UI");
    }

    public Rigidbody2D GetRigidbody()
    {
        return mechInstance != null ? mechInstance.GetRigidbody() : null;
    }

    public bool IsGrounded()
    {
        return mechInstance != null && mechInstance.IsGrounded();
    }

    public float GetFacingDirection()
    {
        return mechInstance != null ? mechInstance.GetFacingDirection() : 1f;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (mechInstance != null)
        {
            mechInstance.OnMove(context.ReadValue<Vector2>());
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && mechInstance != null)
        {
            mechInstance.Jump();
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.performed && mechInstance != null)
        {
            mechInstance.SpecialAttack(this, context);
        }
    }

    public void OnDefensiveAbility(InputAction.CallbackContext context)
    {
        if (context.performed && mechInstance != null)
        {
            mechInstance.DefensiveAbility(this, context);
        }
    } 
    
    public void OnBasicAttack(InputAction.CallbackContext context)
    {
        if (context.performed && mechInstance != null)
        {
            mechInstance.BasicAttack(this, context);
        }
    }

    public void OnMovementAbility(InputAction.CallbackContext context)
    {
        if (mechInstance != null && (context.performed || context.canceled))
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
        if (mechInstance != null)
        {
            mechInstance.SetMovementOverride(isOverriding);
        }
    }

    public void SetMovementOverride(bool isOverriding, float duration)
    {
        if (mechInstance != null)
        {
            mechInstance.SetMovementOverride(isOverriding, duration);
        }
    }

    public void OnPlayerJoined()
    {
        canSubmitAfterTime = Time.time + submitCooldownAfterJoin;

        // Get outline color from player card
        if (playerCard != null)
        {
            outlineColor = playerCard.GetOutlineColor();
        }
    }

    public void OnMechDeath()
    {
        // Relay the mech's death event to external listeners
        onPlayerDeath.Invoke();
    }

    public void Submit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Time.time < canSubmitAfterTime)
            {
                return;
            }

            playerCard.OnSubmit(context);
        }
    }

    public void SwapMechPart(MechPart newPartPrefab)
    {
        if (mechInstance != null)
        {
            mechInstance.SwapPart(newPartPrefab);
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

    public void SetOutlineColor(Color color)
    {
        outlineColor = color;
        if (mechInstance != null)
        {
            mechInstance.SetOutlineColor(color);
        }
    }

    public Color GetOutlineColor()
    {
        return outlineColor;
    }
}

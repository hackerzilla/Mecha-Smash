using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEditor.Rendering;

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

    [Header("User Interface")]
    public PlayerCard playerCard;
    [SerializeField] private float submitCooldownAfterJoin = 1f;

    [Header("Events")]
    public UnityEvent onPlayerDeath = new UnityEvent();

    private PlayerInput playerInput;
    private float canSubmitAfterTime = 0f;
    
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

        // Set up ground check transform for MechMovement
        Transform groundCheck = mechInstance.transform.Find("GroundCheck");
        Assert.NotNull(groundCheck, "need 'GroundCheck' object in Mech prefab");
        MechMovement mechMovement = mechInstance.GetComponent<MechMovement>();
        if (mechMovement != null)
        {
            mechMovement.SetGroundCheck(groundCheck);
        }

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
        if (mechInstance != null)
        {
            Debug.Log("OnSpecialAttack!");
            mechInstance.SpecialAttack(this, context);
        }
    }

    public void OnDefensiveAbility(InputAction.CallbackContext context)
    {
        if (mechInstance != null)
        {
            Debug.Log("OnDefensiveAbility!");
            mechInstance.DefensiveAbility(this, context);
        }
    } 
    
    public void OnBasicAttack(InputAction.CallbackContext context)
    {
        if (mechInstance != null)
        {
            Debug.Log("OnBasicAttack!");
            mechInstance.BasicAttack(this, context);
        }
    }

    public void OnMovementAbility(InputAction.CallbackContext context)
    {
        if (mechInstance != null)
        {
            Debug.Log("OnMovementAbility!");
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
    }

    public void OnMechDeath()
    {
        // Relay the mech's death event to external listeners
        Debug.Log($"[{gameObject.name}] OnMechDeath() called - Time: {Time.time}");
        Debug.Log($"[{gameObject.name}] onPlayerDeath listener count: {onPlayerDeath.GetPersistentEventCount()}");
        Debug.Log($"[{gameObject.name}] Invoking onPlayerDeath event now...");
        onPlayerDeath.Invoke();
        Debug.Log($"[{gameObject.name}] onPlayerDeath event invoked");
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
            Debug.Log($"{gameObject.name} Submit pressed by {GetComponent<PlayerInput>().devices[0].displayName}");
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
}

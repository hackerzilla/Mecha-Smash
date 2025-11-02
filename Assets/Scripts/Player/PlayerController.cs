using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

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
    
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        // playerInput.currentActionMap.Enable();
        if (mechInstance == null)
        {
            Assert.NotNull(mechPrefab);
            mechInstance = Instantiate(mechPrefab, this.transform);
            mechInstance.AssembleMechFromPrefabParts(torsoPrefab, headPrefab, armsPrefab, legsPrefab);
        }
        Assert.NotNull(mechInstance);
        
        rb = mechInstance.GetComponent<Rigidbody2D>();
        
        DisableInputMapping("Player");
        EnableInputMapping("UI"); 
    }

    void Update()
    {
        Rotate();
    }

    private void FixedUpdate()
    {
        // Ground Check
        // isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        // if (isGrounded)
        jumpsRemaining = maxJumps;

        // Movement
        float control = isGrounded ? 1f : airControl;
        // float control = 1f;
        float targetVelocityX = moveInput.x * moveSpeed * control;
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
        
    }
    
    public void Rotate()
    {
        if (moveInput.x > 0.01f)
        {
            // Face Right
            mechInstance.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput.x < -0.01f)
        {
            // Face Left
            mechInstance.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Debug.Log("Move pressed via: " + context.control.device.displayName);
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
    
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerCard.OnNavigate(context);
        }
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
        }
        mechInstance.Jump();
    }
    
    public void SwapMechPart(MechPart newPartPrefab)
    {
        if (mechInstance != null)
        {
            mechInstance.SwapPart(newPartPrefab);
        }
        else
        {
            Debug.Log("For some reason the mechInstance was null (from SwapMechPart in PlayerController on "+ name + ")");
        }
    }
    
    public void OnGameStart()
    {
        DisableInputMapping("UI"); 
        EnableInputMapping("Player");
    }
    public void EnableUIInputMapping()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            var uiMap = playerInput.actions.FindActionMap("UI");
            if (uiMap != null)
            {
                uiMap.Enable();
                Debug.Log($"{gameObject.name}: UI input mapping enabled");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: Could not find 'UI' action map");
            }
        }
    }

    public void DisableInputMapping(string mapName)
    {
        var map = playerInput.actions.FindActionMap(mapName); 
        Debug.Assert(map != null, "Could not find map: " + mapName);
        map.Disable();
        Debug.Log($"{gameObject.name}: Input mapping {mapName} disabled");
    }
    public void EnableInputMapping(string mapName)
    {
        var map = playerInput.actions.FindActionMap(mapName); 
        Debug.Assert(map != null, "Could not find map: " + mapName);
        map.Enable();
        Debug.Log($"{gameObject.name}: Input mapping {mapName} enabled");
    }
}

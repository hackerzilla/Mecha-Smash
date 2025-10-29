using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
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
    public PlayerUI playerUI;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private int jumpsRemaining;
    private bool isGrounded;
    private PlayerInput playerInput;
    
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.currentActionMap.Enable();
        // if (mechInstance == null)
        // {
        //     Assert.NotNull(mechPrefab);
        //     mechInstance = Instantiate(mechPrefab, this.transform);
        //     mechInstance.AssembleMechFromPrefabParts(torsoPrefab, headPrefab, armsPrefab, legsPrefab);
        // }
        // Assert.NotNull(mechInstance);
        
        rb = GetComponent<Rigidbody2D>();
        // Random Color
//         GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);
    }

    void Update()
    {
        // TODO replace this code with gamepad unity input system code
        // float xInput = Input.GetAxisRaw("Horizontal");
        // if (xInput != 0)
        // {
        //     mechInstance.MoveFromInput(xInput);
        //     mechInstance.legsInstance.animator.SetBool("walking", true);
        // }
        // else
        // {
        //     mechInstance.legsInstance.animator.SetBool("walking", false);
        // }
        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     mechInstance.Jump();
        // }
    }
          
    private void FixedUpdate()
    {
        // Ground Check
        // isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        // if (isGrounded)
        //     jumpsRemaining = maxJumps;

        // // Movement
        // float control = isGrounded ? 1f : airControl;
        // float targetVelocityX = moveInput.x * moveSpeed * control;
        // rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
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
        print("Submit called by " + name);
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
        // if (jumpsRemaining > 0)
        // {
        //     rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        //     rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        //     jumpsRemaining -= 1;
        // }
        mechInstance.Jump();
    }
}

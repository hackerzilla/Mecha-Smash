using UnityEngine;
using UnityEngine.InputSystem;

public class ThrusterLegs : LegsPart
{
    [SerializeField] 
    private float thrustForce = 15f;
    [SerializeField] 
    private float maxFuel = 3.0f;
    [SerializeField] 
    private float rechargeRate = 1.0f;

    private float currentFuel;
    private bool isThrusting;
    private PlayerController playerController;

    protected override void Awake()
    {
        base.Awake();
        AbilityName = "Jetpack";
        currentFuel = maxFuel;
        maxJumps = 1;
    }

    void FixedUpdate()
    {
        if (isThrusting && currentFuel > 0 && playerController != null)
        {
            playerController.GetRigidbody().AddForce(Vector2.up * thrustForce);
            currentFuel -= Time.fixedDeltaTime;
        }
    }
    
    void Update() 
    {
        if (playerController == null) return;
        if (!isThrusting)
        {
            if (currentFuel < maxFuel)
            {
                currentFuel += rechargeRate * Time.deltaTime; 
            }
        }
    }

    public override void MovementAbility(PlayerController player, InputAction.CallbackContext context)
    {
        if (playerController == null) playerController = player;

        // When click "LegsAbility"
        if (context.performed)
        {
            if (CanUseAbility() && currentFuel > 0)
            {
                isThrusting = true;
                animator.SetBool("MovementAbilityActive", true); 
                StartCooldown();
            }
        }
        
        // When unclick "LegsAbility"
        if (context.canceled)
        {
            isThrusting = false;
            animator.SetBool("MovementAbilityActive", false);
        }
    }
    
    public override void MovementAbility()
    {}
}

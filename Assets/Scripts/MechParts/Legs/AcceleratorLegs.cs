using UnityEngine;
using UnityEngine.InputSystem;

public class AcceleratorLegs : LegsPart
{
    [SerializeField]
    private float dashForce = 20f;
    [SerializeField]
    private float dashDuration = 0.2f;

    protected override void Awake()
    {
        base.Awake();
        AbilityName = "Tread Dash";
        maxJumps = 1;
    }

    public override void MovementAbility(PlayerController player, InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!CanUseAbility())
            {
                Debug.Log("CoolDown!");
                return;
            }

            Debug.Log(AbilityName + " perform!");
            animator.SetTrigger("MovementAbility"); 

            player.SetMovementOverride(true, dashDuration);
            
            Vector2 dashDir = new Vector2(player.GetFacingDirection(), 0);
            player.GetRigidbody().linearVelocity = new Vector2(dashDir.x * dashForce, 0f);
            StartCooldown();
        }
    }
    public override void MovementAbility()
    {} 
}

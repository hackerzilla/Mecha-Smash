using System.Collections;
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
                return;
            }

            Animator skeletonAnimator = mech.GetSkeletonAnimator();
            if (skeletonAnimator != null)
            {
                skeletonAnimator.SetBool("isDashing", true);
                skeletonAnimator.SetTrigger("dash");
                StartCoroutine(ResetDashingAfterDelay(skeletonAnimator, dashDuration));
            }

            player.SetMovementOverride(true, dashDuration);

            Vector2 dashDir = new Vector2(player.GetFacingDirection(), 0);
            player.GetRigidbody().linearVelocity = new Vector2(dashDir.x * dashForce, 0f);
            StartCooldown();
        }
    }
    public override void MovementAbility()
    {}

    private IEnumerator ResetDashingAfterDelay(Animator animator, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (animator != null)
        {
            animator.SetBool("isDashing", false);
        }
    }
}

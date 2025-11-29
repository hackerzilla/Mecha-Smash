using UnityEngine;
using UnityEngine.InputSystem;

public class HammerHead : HeadPart
{
    [Header("Charge Settings")]
    public float chargeDuration = 0.5f;
    public float chargeForce = 20f;

    [Header("Damage & Knockback")]
    public float damage = 30f;
    public float knockbackForce = 10f;
    public float stunDuration = 1.5f;

    private PlayerController owner;

    public override void SpecialAttack(PlayerController player, InputAction.CallbackContext context)
    {
        HammerHeadCharge(player);
    }

    private void HammerHeadCharge(PlayerController player)
    {
        owner = player;

        // Trigger windup animation - charge will start when OnHeadbuttStart is called by animation event
        Animator skeletonAnimator = mech.GetSkeletonAnimator();
        if (skeletonAnimator != null)
        {
            skeletonAnimator.SetTrigger("headbutt-initiate");
        }

        // Disable movement indefinitely until animation event triggers charge
        player.SetMovementOverride(true);
    }

    public override void OnHeadbuttStart()
    {
        if (owner == null) return;

        // Apply charge velocity when animation event fires (after windup)
        Vector2 direction = new Vector2(owner.GetFacingDirection(), 0);
        Rigidbody2D rb = owner.GetRigidbody();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction.x * chargeForce, rb.linearVelocity.y);
        }

        // Start charge duration timer - movement re-enabled after this
        owner.SetMovementOverride(true, chargeDuration);
    }

    public override void SpecialAttack()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (owner == null) return;

        // Ignore collisions with the owner mech
        if (collision.gameObject == owner || collision.transform.root.gameObject == owner)
        {
            return;
        }

        // Trigger collision animation
        Animator skeletonAnimator = mech.GetSkeletonAnimator();
        if (skeletonAnimator != null)
        {
            skeletonAnimator.SetTrigger("headbutt-collided");
        }

        // Try to find MechHealth component on the collision or its parent
        MechHealth mechHealth = collision.GetComponentInParent<MechHealth>();
        if (mechHealth != null)
        {
            // Deal damage
            mechHealth.TakeDamage(damage);

            // Apply knockback - get components from the mech root
            Rigidbody2D rb = mechHealth.GetComponent<Rigidbody2D>();
            MechMovement mechMovement = mechHealth.GetComponent<MechMovement>();

            if (rb != null && mechMovement != null)
            {
                // Calculate knockback direction and velocity
                Vector2 knockbackDirection = (rb.transform.position - transform.position).normalized;
                Vector2 knockbackVelocity = knockbackDirection * knockbackForce;

                // Disable movement control temporarily
                mechMovement.SetMovementOverride(true);

                // Apply instant velocity change
                rb.linearVelocity = knockbackVelocity;

                // Re-enable movement after charge duration
                mechMovement.SetMovementOverride(true, chargeDuration);
            }
        }
        else
        {
            // Hit a wall stuns the player
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                owner.SetMovementOverride(true);
                owner.SetMovementOverride(true, stunDuration);
            }
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HammerHead : HeadPart
{
    public GameObject headbuttTrigger;
    [Header("Charge Settings")]
    public float chargeDuration = 0.5f;
    public float chargeForce = 20f;

    [Header("Damage & Knockback")]
    public float damage = 30f;
    public float knockbackForce = 10f;
    public float stunDuration = 1.5f;

    private PlayerController owner;

    public override void AttachSprites(Transform headAttachment)
    {
        base.AttachSprites(headAttachment);

        // Reparent headbutt trigger to eyepoint for correct collision position
        if (headbuttTrigger != null && mech.eyePoint != null)
        {
            headbuttTrigger.transform.SetParent(mech.eyePoint);
            headbuttTrigger.transform.localPosition = Vector3.zero;
            headbuttTrigger.SetActive(false);
        }
    }

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

        // Enable headbutt trigger for collision detection
        if (headbuttTrigger != null)
        {
            headbuttTrigger.SetActive(true);
        }

        // Apply charge velocity when animation event fires (after windup)
        Vector2 direction = new Vector2(owner.GetFacingDirection(), 0);
        Rigidbody2D rb = owner.GetRigidbody();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction.x * chargeForce, rb.linearVelocity.y);
        }

        // Start charge duration timer - movement re-enabled after this
        owner.SetMovementOverride(true, chargeDuration);

        // Disable trigger after charge duration
        StartCoroutine(DisableHeadbuttTriggerAfterDelay(chargeDuration));
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

        // Disable headbutt trigger on collision
        DisableHeadbuttTrigger();

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (owner == null) return;

        // Ignore collisions with the owner mech
        if (collision.gameObject == owner.gameObject || collision.transform.root.gameObject == owner.gameObject)
        {
            return;
        }

        // Trigger collision animation for any collision
        Animator skeletonAnimator = mech.GetSkeletonAnimator();
        if (skeletonAnimator != null)
        {
            skeletonAnimator.SetTrigger("headbutt-collided");
        }

        // Disable headbutt trigger on collision
        DisableHeadbuttTrigger();

        // Check if hit a wall - stun the player
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            owner.SetMovementOverride(true);
            owner.SetMovementOverride(true, stunDuration);
        }
    }

    private IEnumerator DisableHeadbuttTriggerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisableHeadbuttTrigger();
    }

    private void DisableHeadbuttTrigger()
    {
        if (headbuttTrigger != null)
        {
            headbuttTrigger.SetActive(false);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (headbuttTrigger != null)
        {
            Destroy(headbuttTrigger);
        }
    }
}

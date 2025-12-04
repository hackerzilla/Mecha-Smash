using System.Collections;
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
    private Collider2D headCollider;
    private HammerHeadTrigger triggerScript;
    private Coroutine chargeCoroutine;

    public override void AttachSprites(Transform headAttachment)
    {
        base.AttachSprites(headAttachment);

        // Get collider and trigger script from headSprite
        if (headSprite != null)
        {
            headCollider = headSprite.GetComponent<Collider2D>();
            triggerScript = headSprite.GetComponent<HammerHeadTrigger>();

            if (triggerScript != null)
            {
                triggerScript.owner = mech.gameObject;
                triggerScript.hammerHead = this;
            }

            if (headCollider != null)
                headCollider.enabled = false;
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

        // Enable collider and reset hit state
        if (triggerScript != null) triggerScript.ResetHit();
        if (headCollider != null) headCollider.enabled = true;

        // Apply charge velocity when animation event fires (after windup)
        Vector2 direction = new Vector2(owner.GetFacingDirection(), 0);
        Rigidbody2D rb = owner.GetRigidbody();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction.x * chargeForce, rb.linearVelocity.y);
        }

        // Start charge duration timer - movement re-enabled after this
        owner.SetMovementOverride(true, chargeDuration);

        // Disable collider after charge duration if no collision occurred
        if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
        chargeCoroutine = StartCoroutine(DisableColliderAfterDelay(chargeDuration));
    }

    public override void SpecialAttack()
    {
    }

    public void OnHeadbuttCollision(Collider2D collision)
    {
        // Stop the auto-disable coroutine
        if (chargeCoroutine != null)
        {
            StopCoroutine(chargeCoroutine);
            chargeCoroutine = null;
        }

        // Trigger collision animation
        Animator skeletonAnimator = mech.GetSkeletonAnimator();
        if (skeletonAnimator != null)
        {
            skeletonAnimator.SetTrigger("headbutt-collided");
        }

        // Disable collider
        if (headCollider != null) headCollider.enabled = false;

        // Stop the charge momentum
        if (owner != null)
        {
            Rigidbody2D ownerRb = owner.GetRigidbody();
            if (ownerRb != null)
            {
                ownerRb.linearVelocity = new Vector2(0, ownerRb.linearVelocity.y);
            }
        }

        // Try to find MechHealth on collision target
        MechHealth mechHealth = collision.GetComponentInParent<MechHealth>();
        if (mechHealth != null)
        {
            // Deal damage
            mechHealth.TakeDamage(damage);

            // Apply knockback
            Rigidbody2D targetRb = mechHealth.GetComponent<Rigidbody2D>();
            MechMovement targetMovement = mechHealth.GetComponent<MechMovement>();

            if (targetRb != null && targetMovement != null)
            {
                Vector2 knockbackDirection = (targetRb.transform.position - transform.position).normalized;
                targetMovement.SetMovementOverride(true, stunDuration);
                targetRb.linearVelocity = knockbackDirection * knockbackForce;
            }
        }
        else
        {
            // Hit a wall - stun the owner
            if (owner != null)
            {
                owner.SetMovementOverride(true, stunDuration);
            }
        }
    }

    private IEnumerator DisableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (headCollider != null) headCollider.enabled = false;
        chargeCoroutine = null;
    }
}

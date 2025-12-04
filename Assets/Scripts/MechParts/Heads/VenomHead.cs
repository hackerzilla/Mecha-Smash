using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class VenomHead : HeadPart
{
    public float biteDuration = 0.5f;
    public float damagePerTick = 10f;
    public float tickInterval = 0.5f;
    public int tickCount = 4;
    public float healPerTick = 5f;

    private PlayerController owner;
    private Collider2D headCollider;
    private VenomTrigger triggerScript;

    public override void AttachSprites(Transform headAttachment)
    {
        base.AttachSprites(headAttachment);

        // Get collider and trigger script from headSprite
        if (headSprite != null)
        {
            headCollider = headSprite.GetComponent<Collider2D>();
            triggerScript = headSprite.GetComponent<VenomTrigger>();

            if (triggerScript != null)
            {
                triggerScript.owner = mech.gameObject;
                triggerScript.venomHead = this;
            }

            if (headCollider != null)
                headCollider.enabled = false;
        }
    }

    public override void SpecialAttack(PlayerController player, InputAction.CallbackContext context)
    {
        BitePlayer(player);
    }

    public override void SpecialAttack()
    {
        
    }

    private void BitePlayer(PlayerController player)
    {
        owner = player;
        // Trigger bite animation on skeleton animator
        Animator skeletonAnimator = mech.GetSkeletonAnimator();
        if (skeletonAnimator != null)
        {
            // Debug.Log("in bite, found skeleton animator");
            skeletonAnimator.SetTrigger("bite");
        }

        // Disable movement during bite
        player.SetMovementOverride(true, biteDuration);
    }

    public override void OnBiteStart()
    {
        // Reset hit state for new bite
        if (triggerScript != null) triggerScript.ResetHit();

        // Enable head collider when animation event fires
        if (headCollider != null) headCollider.enabled = true;

        // Disable after bite duration
        StartCoroutine(DisableColliderAfterDelay());
    }

    public void OnVenomHit(MechHealth target)
    {
        // Apply venom DoT to target and heal self
        StartCoroutine(ApplyVenomDoT(target));
        StartCoroutine(HealVenomDoT());

        // Apply brief knockback
        MechMovement targetMovement = target.GetComponent<MechMovement>();
        if (targetMovement != null)
        {
            targetMovement.SetMovementOverride(true, 0.2f);
        }
    }

    private IEnumerator DisableColliderAfterDelay()
    {
        yield return new WaitForSeconds(biteDuration);
        if (headCollider != null) headCollider.enabled = false;
    }

    public IEnumerator ApplyVenomDoT(MechHealth target)
    {
        int ticks = 0;
        while (ticks < tickCount && target != null)
        {
            target.TakeDamage(damagePerTick);
            ticks++;
            yield return new WaitForSeconds(tickInterval);
        }
    }

    public IEnumerator HealVenomDoT()
    {
        int ticks = 0;
        while (ticks < tickCount && owner != null)
        {
            owner.mechInstance.GetComponent<MechHealth>().Heal(healPerTick);
            ticks++;
            yield return new WaitForSeconds(tickInterval);
        }
    }

}

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class VenomHead : HeadPart
{
    public Venom venom;
    public float biteDuration = 0.5f;
    public float damagePerTick = 10f;
    public float tickInterval = 0.5f;
    public int tickCount = 4;
    public float healPerTick = 5f;

    private PlayerController owner;

    public override void AttachSprites(Transform headAttachment)
    {
        base.AttachSprites(headAttachment);

        // Reparent venom hitbox to eyepoint for correct collision position
        if (venom != null && mech.eyePoint != null)
        {
            venom.transform.SetParent(mech.eyePoint);
            venom.transform.localPosition = Vector3.zero;
            venom.SetOwner(mech.gameObject);

            // Start disabled
            venom.GetComponent<BoxCollider2D>().enabled = false;
            venom.GetComponent<SpriteRenderer>().enabled = false;
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
        // Enable venom hitbox when animation event fires
        venom.GetComponent<BoxCollider2D>().enabled = true;
        venom.GetComponent<SpriteRenderer>().enabled = true;

        // Disable after bite duration
        StartCoroutine(DisableVenomAfterDelay());
    }

    private IEnumerator DisableVenomAfterDelay()
    {
        yield return new WaitForSeconds(biteDuration);
        venom.GetComponent<BoxCollider2D>().enabled = false;
        venom.GetComponent<SpriteRenderer>().enabled = false;
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (venom != null)
        {
            Destroy(venom.gameObject);
        }
    }
}

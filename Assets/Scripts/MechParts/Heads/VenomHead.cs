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
    private MechController mechController;

    protected override void Awake()
    {
        base.Awake();
        mechController = transform.root.GetComponent<PlayerController>()?.mechInstance;
        venom.SetOwner(mechController.gameObject);
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

        Debug.Log("VenomHead bite!");
        animator.SetTrigger("SpecialAttack");

        // Disable Controller
        player.SetMovementOverride(true, biteDuration);

        StartCoroutine(VenomOn());
    }

    private IEnumerator VenomOn()
    {
        venom.GetComponent<BoxCollider2D>().enabled = true;
        venom.GetComponent<SpriteRenderer>().enabled = true;
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
}
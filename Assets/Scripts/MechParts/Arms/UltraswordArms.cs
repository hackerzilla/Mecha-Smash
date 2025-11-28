using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class UltraswordArms : ArmsPart 
{
    [SerializeField] private GameObject swordHitBox; // sword that has sword.cs
    [SerializeField] private float windUpTime = 0.8f; // attack delay
    [SerializeField] private float attackDuration = 0.5f;
    private bool isAttacking = false;

    protected override void Awake() {
        base.Awake();
        if(swordHitBox != null)
        {
            swordHitBox.SetActive(false);
        }
    }

    override public void BasicAttack(PlayerController player, InputAction.CallbackContext context)
    {
        if(context.performed && !isAttacking)
        {
            BasicAttack();
        }
    }

    override public void BasicAttack()
    {
        if(mech == null)
        {
            mech = transform.root.GetComponent<PlayerController>().mechInstance;
            StartCoroutine(SlashRoutine());
        }
    }

    private IEnumerator SlashRoutine()
    {
        isAttacking = true;

        // attack delay
        animator.SetTrigger("WindUp");
        Debug.Log("Ultrasonic Piezosword Charging");

        // remove mech control during attack delay time
        mech.SetMovementOverride(true, windUpTime + attackDuration);
        yield return new WaitForSeconds(windUpTime);

        // attack
        animator.SetTrigger("BasicAttack");
        Debug.Log("Ultrasonic Piezosword Slam");

        if(swordHitBox != null)
        {
            // set owner
            StunBlade bladeScript = swordHitBox.GetComponent<StunBlade>();
            if(bladeScript != null)
            {
                bladeScript.owner = mech.gameObject;
            }

            swordHitBox.SetActive(true); // turn on the blade
            yield return new WaitForSeconds(attackDuration);
            swordHitBox.SetActive(false); // turn off the blade
        }

        isAttacking = false;
    }
}

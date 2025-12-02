using UnityEngine;
using UnityEngine.InputSystem;

public class UltraswordArms : ArmsPart
{
    [SerializeField] private GameObject swordHitBox;
    private bool isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        if (swordHitBox != null)
        {
            swordHitBox.SetActive(false);
        }
    }

    public override void BasicAttack(PlayerController player, InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking)
        {
            isAttacking = true;

            Debug.Log("Attacking");
            Animator skeletonAnimator = mech.GetSkeletonAnimator();
            if (skeletonAnimator != null)
            {
                skeletonAnimator.SetTrigger("sword-swing");
            }
        }
    }

    public override void BasicAttack() { }

    public override void OnSwordSwingHit()
    {
        if (swordHitBox != null)
        {
            StunBlade bladeScript = swordHitBox.GetComponent<StunBlade>();
            if (bladeScript != null)
            {
                bladeScript.owner = mech.gameObject;
            }

            swordHitBox.SetActive(true);
        }
    }

    public override void OnSwordSwingEnd()
    {
        if (swordHitBox != null)
        {
            swordHitBox.SetActive(false);
        }

        isAttacking = false;
    }
}

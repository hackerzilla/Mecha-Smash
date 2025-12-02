using UnityEngine;
using UnityEngine.InputSystem;

public class UltraswordArms : ArmsPart
{
    [SerializeField] private GameObject swordParent;
    private Collider2D swordCollider;
    private bool isAttacking = false;

    public override void AttachSprites(Transform leftHandAttachment, Transform rightHandAttachment)
    {
        base.AttachSprites(leftHandAttachment, rightHandAttachment);

        // Reparent sword parent to right hand attachment point
        if (swordParent != null && rightHandAttachment != null)
        {
            swordParent.transform.SetParent(rightHandAttachment);
            swordParent.transform.localPosition = Vector3.zero;

            // Set owner for StunBlade (on child)
            StunBlade bladeScript = swordParent.GetComponentInChildren<StunBlade>();
            if (bladeScript != null)
            {
                bladeScript.owner = mech.gameObject;
            }

            // Cache collider (on child) and start with it disabled
            swordCollider = swordParent.GetComponentInChildren<Collider2D>();
            if (swordCollider != null)
            {
                swordCollider.enabled = false;
            }
        }
    }

    public override void BasicAttack(PlayerController player, InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking)
        {
            isAttacking = true;

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
        if (swordCollider != null)
        {
            swordCollider.enabled = true;
        }
    }

    public override void OnSwordSwingEnd()
    {
        if (swordCollider != null)
        {
            swordCollider.enabled = false;
        }

        isAttacking = false;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (swordParent != null)
        {
            Destroy(swordParent);
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class UltraswordArms : ArmsPart
{
    [SerializeField] private StunBlade stunBlade;
    private Collider2D swordCollider;
    private bool isAttacking = false;

    public override void AttachSprites(Transform leftHandAttachment, Transform rightHandAttachment)
    {
        base.AttachSprites(leftHandAttachment, rightHandAttachment);

        if (stunBlade != null && rightHandAttachment != null)
        {
            // Reparent stunBlade to right hand attachment point
            stunBlade.transform.SetParent(rightHandAttachment);
            stunBlade.transform.localPosition = Vector3.zero;

            // Set owner
            stunBlade.owner = mech.gameObject;

            // Cache collider and start with it disabled
            swordCollider = stunBlade.GetComponent<Collider2D>();
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
        if (stunBlade != null)
        {
            stunBlade.ResetHit();
        }
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
        if (stunBlade != null)
        {
            Destroy(stunBlade.gameObject);
        }
    }
}

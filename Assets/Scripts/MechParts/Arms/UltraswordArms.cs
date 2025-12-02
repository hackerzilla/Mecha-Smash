using UnityEngine;
using UnityEngine.InputSystem;

public class UltraswordArms : ArmsPart
{
    [SerializeField] private GameObject swordSpriteAndHitbox;
    private Collider2D swordCollider;
    private bool isAttacking = false;

    public override void AttachSprites(Transform leftHandAttachment, Transform rightHandAttachment)
    {
        base.AttachSprites(leftHandAttachment, rightHandAttachment);

        // Reparent sword to right hand attachment point
        if (swordSpriteAndHitbox != null && rightHandAttachment != null)
        {
            swordSpriteAndHitbox.transform.SetParent(rightHandAttachment);
            swordSpriteAndHitbox.transform.localPosition = Vector3.zero;

            // Set owner for StunBlade
            StunBlade bladeScript = swordSpriteAndHitbox.GetComponent<StunBlade>();
            if (bladeScript != null)
            {
                bladeScript.owner = mech.gameObject;
            }

            // Cache collider and start with it disabled (sprite stays visible)
            swordCollider = swordSpriteAndHitbox.GetComponent<Collider2D>();
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
        if (swordSpriteAndHitbox != null)
        {
            Destroy(swordSpriteAndHitbox);
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class StrikerArms : ArmsPart
{
    private Collider2D leftFistCollider;
    private Collider2D rightFistCollider;
    private bool isAttacking;

    public override void AttachSprites(Transform leftHandAttachment, Transform rightHandAttachment)
    {
        base.AttachSprites(leftHandAttachment, rightHandAttachment);

        // Cache colliders and set owner on fist scripts
        if (leftHandSprite != null)
        {
            leftFistCollider = leftHandSprite.GetComponent<Collider2D>();
            StrikerFist leftFist = leftHandSprite.GetComponent<StrikerFist>();
            if (leftFist != null) leftFist.owner = mech.gameObject;
        }
        if (rightHandSprite != null)
        {
            rightFistCollider = rightHandSprite.GetComponent<Collider2D>();
            StrikerFist rightFist = rightHandSprite.GetComponent<StrikerFist>();
            if (rightFist != null) rightFist.owner = mech.gameObject;
        }

        // Start with colliders disabled
        if (leftFistCollider != null) leftFistCollider.enabled = false;
        if (rightFistCollider != null) rightFistCollider.enabled = false;
    }

    public override void BasicAttack(PlayerController player, InputAction.CallbackContext context)
    {
        if (!context.performed || isAttacking) return;
        BasicAttack();
    }

    public override void BasicAttack()
    {
        isAttacking = true;
        Animator anim = mech.skeletonRig.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("punch");
        }
    }

    // Animation event - enable left fist hitbox
    public override void OnLeftPunchHit()
    {
        if (leftHandSprite != null)
        {
            StrikerFist leftFist = leftHandSprite.GetComponent<StrikerFist>();
            if (leftFist != null) leftFist.ResetHit();
        }
        if (leftFistCollider != null) leftFistCollider.enabled = true;
    }

    // Animation event - disable left fist hitbox
    public override void OnLeftPunchEnd()
    {
        if (leftFistCollider != null) leftFistCollider.enabled = false;
    }

    // Animation event - enable right fist hitbox
    public override void OnRightPunchHit()
    {
        if (rightHandSprite != null)
        {
            StrikerFist rightFist = rightHandSprite.GetComponent<StrikerFist>();
            if (rightFist != null) rightFist.ResetHit();
        }
        if (rightFistCollider != null) rightFistCollider.enabled = true;
    }

    // Animation event - disable right fist hitbox and end attack
    public override void OnRightPunchEnd()
    {
        if (rightFistCollider != null) rightFistCollider.enabled = false;
        isAttacking = false;
    }
}

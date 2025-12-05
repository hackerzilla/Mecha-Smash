using UnityEngine;
using UnityEngine.InputSystem;

abstract public class ArmsPart : MechPart
{
    // Sprite management for skeleton rig system
    [Header("Sprite Configuration")]
    [SerializeField] protected GameObject leftHandSprite;
    [SerializeField] protected GameObject rightHandSprite;

    abstract public void BasicAttack(PlayerController player, InputAction.CallbackContext context);
    abstract public void BasicAttack();

    /// <summary>
    /// Called by animation event to fire the right hand gun. Override in arms that use guns.
    /// </summary>
    public virtual void ShootRightHandGun() { }

    /// <summary>
    /// Called by animation event to fire the left hand gun. Override in arms that use guns.
    /// </summary>
    public virtual void ShootLeftHandGun() { }

    /// <summary>
    /// Called by animation event when shoot animation completes. Override to clear shooting state.
    /// </summary>
    public virtual void OnShootComplete() { }

    /// <summary>
    /// Called by animation event when sword swing hits. Override in arms that use swords.
    /// </summary>
    public virtual void OnSwordSwingHit() { }

    /// <summary>
    /// Called by animation event when sword swing ends. Override in arms that use swords.
    /// </summary>
    public virtual void OnSwordSwingEnd() { }

    /// <summary>
    /// Called by animation event when left punch hitbox should activate.
    /// </summary>
    public virtual void OnLeftPunchHit() { }

    /// <summary>
    /// Called by animation event when left punch ends.
    /// </summary>
    public virtual void OnLeftPunchEnd() { }

    /// <summary>
    /// Called by animation event when right punch hitbox should activate.
    /// </summary>
    public virtual void OnRightPunchHit() { }

    /// <summary>
    /// Called by animation event when right punch ends.
    /// </summary>
    public virtual void OnRightPunchEnd() { }

    /// <summary>
    /// Attaches the arm sprites to the skeleton rig at the specified hand attachment points.
    /// Called by MechController during part assembly.
    /// </summary>
    public virtual void AttachSprites(Transform leftHandAttachment, Transform rightHandAttachment)
    {
        // Reparent left hand sprite
        if (leftHandSprite != null && leftHandAttachment != null)
        {
            leftHandSprite.transform.SetParent(leftHandAttachment);
            leftHandSprite.transform.localPosition = Vector2.zero;
            leftHandSprite.transform.localScale = Vector3.one;
        }

        // Reparent right hand sprite
        if (rightHandSprite != null && rightHandAttachment != null)
        {
            rightHandSprite.transform.SetParent(rightHandAttachment);
            rightHandSprite.transform.localPosition = Vector2.zero;
            rightHandSprite.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// Cleanup sprites when this part is destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (leftHandSprite != null)
        {
            Destroy(leftHandSprite);
        }
        if (rightHandSprite != null)
        {
            Destroy(rightHandSprite);
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

abstract public class HeadPart : MechPart
{
    // Sprite management for skeleton rig system
    [Header("Sprite Configuration")]
    [SerializeField] protected GameObject headSprite;

    abstract public void SpecialAttack(PlayerController player, InputAction.CallbackContext context);
    abstract public void SpecialAttack();

    /// <summary>
    /// Called by animation event when headbutt windup ends. Override in heads that use headbutt.
    /// </summary>
    public virtual void OnHeadbuttStart() { }

    /// <summary>
    /// Called by animation event when bite animation reaches the bite frame. Override in heads that use bite.
    /// </summary>
    public virtual void OnBiteStart() { }

    /// <summary>
    /// Attaches the head sprite to the skeleton rig at the specified attachment point.
    /// Called by MechController during part assembly.
    /// </summary>
    public virtual void AttachSprites(Transform headAttachment)
    {
        // Reparent sprite to attachment point
        if (headSprite != null && headAttachment != null)
        {
            headSprite.transform.SetParent(headAttachment);
            headSprite.transform.localPosition = Vector2.zero;
            headSprite.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// Cleanup sprite when this part is destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (headSprite != null)
        {
            Destroy(headSprite);
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

abstract public class LegsPart : MechPart
{
    [SerializeField]
    protected string AbilityName;
    [SerializeField]
    protected float cooldown = 1.0f;

    public float moveSpeed = 8f;
    public int maxJumps = 1;

    private float lastUseTime = -1.0f;

    // Sprite management for skeleton rig system
    [Header("Sprite Configuration")]
    [SerializeField] protected GameObject leftFootSprite;
    [SerializeField] protected GameObject rightFootSprite;

    abstract public void MovementAbility(PlayerController player, InputAction.CallbackContext context);

    public virtual bool CanUseAbility()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    protected void StartCooldown()
    {
        lastUseTime = Time.time;
    }
    abstract public void MovementAbility();

    /// <summary>
    /// Attaches the leg sprites to the skeleton rig at the specified foot attachment points.
    /// Called by MechController during part assembly.
    /// </summary>
    public virtual void AttachSprites(Transform leftFootAttachment, Transform rightFootAttachment)
    {
        // Reparent left foot sprite
        if (leftFootSprite != null && leftFootAttachment != null)
        {
            leftFootSprite.transform.SetParent(leftFootAttachment);
            leftFootSprite.transform.localPosition = Vector2.zero;
            leftFootSprite.transform.localScale = Vector3.one;
        }

        // Reparent right foot sprite
        if (rightFootSprite != null && rightFootAttachment != null)
        {
            rightFootSprite.transform.SetParent(rightFootAttachment);
            rightFootSprite.transform.localPosition = Vector2.zero;
            rightFootSprite.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// Cleanup sprites when this part is destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (leftFootSprite != null)
        {
            Destroy(leftFootSprite);
        }
        if (rightFootSprite != null)
        {
            Destroy(rightFootSprite);
        }
    }
}

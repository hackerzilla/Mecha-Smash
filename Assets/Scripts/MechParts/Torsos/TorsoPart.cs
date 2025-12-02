using UnityEngine;
using UnityEngine.InputSystem;

abstract public class TorsoPart : MechPart
{
    [SerializeField] protected float cooldown = 1.0f;
    protected string AbilityName;
    protected float lastUseTime = -1.0f;

    // Sprite management for skeleton rig system
    [Header("Sprite Configuration")]
    [SerializeField] protected GameObject chestSprite;
    [SerializeField] protected GameObject coreSprite;
    [SerializeField] protected GameObject leftShoulderSprite;
    [SerializeField] protected GameObject rightShoulderSprite;

    abstract public void DefensiveAbility(PlayerController player, InputAction.CallbackContext context);

    public virtual bool CanUseAbility()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    protected void StartCooldown()
    {
        lastUseTime = Time.time;
    }
    abstract public void DefensiveAbility();

    /// <summary>
    /// Attaches the torso sprites to the skeleton rig at the specified attachment points.
    /// Called by MechController during part assembly.
    /// </summary>
    public virtual void AttachSprites(Transform chestAttachment, Transform coreAttachment,
                                       Transform leftShoulderAttachment, Transform rightShoulderAttachment)
    {
        if (chestSprite != null && chestAttachment != null)
        {
            chestSprite.transform.SetParent(chestAttachment);
            chestSprite.transform.localPosition = Vector2.zero;
        }

        if (coreSprite != null && coreAttachment != null)
        {
            coreSprite.transform.SetParent(coreAttachment);
            coreSprite.transform.localPosition = Vector2.zero;
        }

        if (leftShoulderSprite != null && leftShoulderAttachment != null)
        {
            leftShoulderSprite.transform.SetParent(leftShoulderAttachment);
            leftShoulderSprite.transform.localPosition = Vector2.zero;
        }

        if (rightShoulderSprite != null && rightShoulderAttachment != null)
        {
            rightShoulderSprite.transform.SetParent(rightShoulderAttachment);
            rightShoulderSprite.transform.localPosition = Vector2.zero;
        }
    }

    /// <summary>
    /// Cleanup sprites when this part is destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (chestSprite != null) Destroy(chestSprite);
        if (coreSprite != null) Destroy(coreSprite);
        if (leftShoulderSprite != null) Destroy(leftShoulderSprite);
        if (rightShoulderSprite != null) Destroy(rightShoulderSprite);
    }
}

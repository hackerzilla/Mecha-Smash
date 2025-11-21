using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

abstract public class TorsoPart : MechPart
{
    [SerializeField] protected float cooldown = 1.0f;
    protected string AbilityName;
    protected float lastUseTime = -1.0f;

    // Sprite management for skeleton rig system
    [Header("Sprite Configuration")]
    [SerializeField] protected List<GameObject> torsoSprites = new List<GameObject>();

    // Legacy attachment points - no longer used with skeleton rig system
    // Kept for backwards compatibility with existing prefabs
    public Transform headAttachmentPoint;
    public Transform armsAttachmentPoint;
    public Transform legsAttachmentPoint;

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
    /// Attaches the torso sprites to the skeleton rig at the specified attachment point.
    /// Called by MechController during part assembly.
    /// </summary>
    public virtual void AttachSprites(Transform torsoAttachment)
    {
        // Reparent all torso sprites to attachment point
        if (torsoAttachment != null)
        {
            foreach (GameObject sprite in torsoSprites)
            {
                if (sprite != null)
                {
                    sprite.transform.SetParent(torsoAttachment);
                    sprite.transform.localPosition = Vector2.zero;
                }
            }
        }
    }

    /// <summary>
    /// Cleanup sprites when this part is destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        foreach (GameObject sprite in torsoSprites)
        {
            if (sprite != null)
            {
                Destroy(sprite);
            }
        }
    }
}

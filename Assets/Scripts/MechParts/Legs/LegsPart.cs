using System.IO.Compression;
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
    [SerializeField] protected GameObject leftFootSpritePrefab;
    [SerializeField] protected GameObject rightFootSpritePrefab;
    protected GameObject leftFootSpriteInstance;
    protected GameObject rightFootSpriteInstance;

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
        // Clean up existing sprites if any
        if (leftFootSpriteInstance != null)
        {
            Destroy(leftFootSpriteInstance);
        }
        if (rightFootSpriteInstance != null)
        {
            Destroy(rightFootSpriteInstance);
        }

        // Instantiate left foot sprite
        if (leftFootSpritePrefab != null && leftFootAttachment != null)
        {
            leftFootSpriteInstance = Instantiate(leftFootSpritePrefab, leftFootAttachment);
            leftFootSpriteInstance.transform.localScale = Vector3.one;
            leftFootSpriteInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        }

        // Instantiate right foot sprite
        if (rightFootSpritePrefab != null && rightFootAttachment != null)
        {
            rightFootSpriteInstance = Instantiate(rightFootSpritePrefab, rightFootAttachment);
            rightFootSpriteInstance.transform.localScale = Vector3.one;
            rightFootSpriteInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        }
    }

    /// <summary>
    /// Cleanup sprite instances when this part is destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (leftFootSpriteInstance != null)
        {
            Destroy(leftFootSpriteInstance);
        }
        if (rightFootSpriteInstance != null)
        {
            Destroy(rightFootSpriteInstance);
        }
    }
}

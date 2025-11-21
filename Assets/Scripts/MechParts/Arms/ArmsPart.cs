using UnityEngine;
using UnityEngine.InputSystem;

abstract public class ArmsPart : MechPart
{
    // Sprite management for skeleton rig system
    [Header("Sprite Configuration")]
    [SerializeField] protected GameObject leftHandSpritePrefab;
    [SerializeField] protected GameObject rightHandSpritePrefab;
    protected GameObject leftHandSpriteInstance;
    protected GameObject rightHandSpriteInstance;

    abstract public void BasicAttack(PlayerController player, InputAction.CallbackContext context);
    abstract public void BasicAttack();

    /// <summary>
    /// Attaches the arm sprites to the skeleton rig at the specified hand attachment points.
    /// Called by MechController during part assembly.
    /// </summary>
    public virtual void AttachSprites(Transform leftHandAttachment, Transform rightHandAttachment)
    {
        // Clean up existing sprites if any
        if (leftHandSpriteInstance != null)
        {
            Destroy(leftHandSpriteInstance);
        }
        if (rightHandSpriteInstance != null)
        {
            Destroy(rightHandSpriteInstance);
        }

        // Instantiate left hand sprite
        if (leftHandSpritePrefab != null && leftHandAttachment != null)
        {
            leftHandSpriteInstance = Instantiate(leftHandSpritePrefab, leftHandAttachment);
            leftHandSpriteInstance.transform.localScale = Vector3.one;
            leftHandSpriteInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        }

        // Instantiate right hand sprite
        if (rightHandSpritePrefab != null && rightHandAttachment != null)
        {
            rightHandSpriteInstance = Instantiate(rightHandSpritePrefab, rightHandAttachment);
            rightHandSpriteInstance.transform.localScale = Vector3.one;
            rightHandSpriteInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        }
    }

    /// <summary>
    /// Cleanup sprite instances when this part is destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (leftHandSpriteInstance != null)
        {
            Destroy(leftHandSpriteInstance);
        }
        if (rightHandSpriteInstance != null)
        {
            Destroy(rightHandSpriteInstance);
        }
    }
}

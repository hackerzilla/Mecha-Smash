using UnityEngine;
using UnityEngine.InputSystem;

abstract public class HeadPart : MechPart
{
    // Sprite management for skeleton rig system
    [Header("Sprite Configuration")]
    [SerializeField] protected GameObject headSpritePrefab;
    protected GameObject headSpriteInstance;

    abstract public void SpecialAttack(PlayerController player, InputAction.CallbackContext context);
    abstract public void SpecialAttack();

    /// <summary>
    /// Attaches the head sprite to the skeleton rig at the specified attachment point.
    /// Called by MechController during part assembly.
    /// </summary>
    public virtual void AttachSprites(Transform headAttachment)
    {
        // Clean up existing sprite if any
        if (headSpriteInstance != null)
        {
            Destroy(headSpriteInstance);
        }

        // Instantiate sprite at attachment point
        if (headSpritePrefab != null && headAttachment != null)
        {
            headSpriteInstance = Instantiate(headSpritePrefab, headAttachment);
            headSpriteInstance.transform.localScale = Vector3.one;
            headSpriteInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        }
    }

    /// <summary>
    /// Cleanup sprite instances when this part is destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (headSpriteInstance != null)
        {
            Destroy(headSpriteInstance);
        }
    }
}

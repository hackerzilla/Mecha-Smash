using UnityEngine;

/// <summary>
/// Bridge script for Animation Events. Place on the skeleton child object (same GameObject as Animator).
/// Forwards Animation Events to the parent MechController.
/// </summary>
public class MechAnimationEvents : MonoBehaviour
{
    private MechController mechController;

    void Awake()
    {
        mechController = GetComponentInParent<MechController>();
    }

    /// <summary>
    /// Called by Animation Event on jump animation to apply jump force.
    /// </summary>
    public void OnJumpEvent()
    {
        if (mechController != null)
        {
            mechController.OnJumpAnimationEvent();
        }
    }

    /// <summary>
    /// Called by Animation Event on shoot animation to fire the right hand gun.
    /// </summary>
    public void OnShootRightHandGun()
    {
        if (mechController != null)
        {
            mechController.OnShootRightHandGunEvent();
        }
    }

    /// <summary>
    /// Called by Animation Event on shoot animation to fire the left hand gun.
    /// </summary>
    public void OnShootLeftHandGun()
    {
        if (mechController != null)
        {
            mechController.OnShootLeftHandGunEvent();
        }
    }

    /// <summary>
    /// Called by Animation Event when shoot animation completes.
    /// </summary>
    public void OnShootAnimationComplete()
    {
        if (mechController != null)
        {
            mechController.OnShootAnimationCompleteEvent();
        }
    }
}

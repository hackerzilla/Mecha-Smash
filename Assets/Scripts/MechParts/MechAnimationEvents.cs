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
}

using UnityEngine;

/// <summary>
/// Bridge script for Animation Events. Place on the skeleton child object (same GameObject as Animator).
/// Forwards Animation Events to the parent MechController.
/// </summary>
public class MechAnimationEvents : MonoBehaviour
{
    [Header("Skeleton Sprites")]
    public GameObject leftFootSprite;
    public GameObject rightFootSprite;

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

    /// <summary>
    /// Called by Animation Event when headbutt windup ends and charge should begin.
    /// </summary>
    public void OnHeadbuttStart()
    {
        if (mechController != null)
        {
            mechController.OnHeadbuttStartEvent();
        }
    }

    /// <summary>
    /// Called by Animation Event when bite animation reaches the bite frame.
    /// </summary>
    public void OnBiteStart()
    {
        if (mechController != null)
        {
            mechController.OnBiteStartEvent();
        }
    }

    /// <summary>
    /// Called by Animation Event when sword swing hits.
    /// </summary>
    public void OnSwordSwingHit()
    {
        if (mechController != null)
        {
            mechController.OnSwordSwingHitEvent();
        }
    }

    /// <summary>
    /// Called by Animation Event when sword swing ends.
    /// </summary>
    public void OnSwordSwingEnd()
    {
        if (mechController != null)
        {
            mechController.OnSwordSwingEndEvent();
        }
    }

    /// <summary>
    /// Called by Animation Event when left punch hitbox should activate.
    /// </summary>
    public void OnLeftPunchHit()
    {
        if (mechController != null)
        {
            mechController.OnLeftPunchHitEvent();
        }
    }

    /// <summary>
    /// Called by Animation Event when left punch ends.
    /// </summary>
    public void OnLeftPunchEnd()
    {
        if (mechController != null)
        {
            mechController.OnLeftPunchEndEvent();
        }
    }

    /// <summary>
    /// Called by Animation Event when right punch hitbox should activate.
    /// </summary>
    public void OnRightPunchHit()
    {
        if (mechController != null)
        {
            mechController.OnRightPunchHitEvent();
        }
    }

    /// <summary>
    /// Called by Animation Event when right punch ends.
    /// </summary>
    public void OnRightPunchEnd()
    {
        if (mechController != null)
        {
            mechController.OnRightPunchEndEvent();
        }
    }
}

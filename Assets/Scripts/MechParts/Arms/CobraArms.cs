using UnityEngine;
using UnityEngine.InputSystem;

public class CobraArms : ArmsPart
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    
    [SerializeField] private AudioSource gunShot;

    private bool isShooting = false;

    public override void BasicAttack(PlayerController player, InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isShooting) return;
        BasicAttack();
    }

    public override void BasicAttack()
    {
        if (isShooting) return;

        isShooting = true;

        // Trigger shoot animation on skeleton - animation events will call ShootRightHandGun/ShootLeftHandGun
        Animator skeletonAnimator = mech.GetSkeletonAnimator();
        if (skeletonAnimator != null)
        {
            skeletonAnimator.SetTrigger("shoot-pistols");
        }
    }

    public override void ShootRightHandGun()
    {
        FireProjectile(mech.rightHandAttachment);
    }

    public override void ShootLeftHandGun()
    {
        FireProjectile(mech.leftHandAttachment);
    }

    public override void OnShootComplete()
    {
        isShooting = false;
    }

    private void FireProjectile(Transform handAttachment)
    {
        if (projectilePrefab == null || handAttachment == null || mech == null)
        {
            Debug.LogWarning("CobraArms: Missing required references for firing");
            return;
        }

        Vector2 direction = Vector2.right * mech.GetFacingDirection();
        Vector3 spawnPosition = handAttachment.position;

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Cannon cannon = projectile.GetComponent<Cannon>();
        if (cannon != null)
        {
            cannon.direction = direction;
            cannon.SetOwner(mech.gameObject);
        }
        gunShot.Play();
    }
}

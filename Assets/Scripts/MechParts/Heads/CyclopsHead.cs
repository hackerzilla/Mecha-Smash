using UnityEngine;
using UnityEngine.InputSystem;

public class CyclopsHead : HeadPart
{
    public Transform eyePoint;
    public GameObject laserPrefab;
    private LaserBeam spawnedLaser;

    private void Start()
    {
        GameObject laser = Instantiate(laserPrefab, eyePoint.position, eyePoint.rotation);
        spawnedLaser = laser.GetComponent<LaserBeam>();
        laser.transform.SetParent(eyePoint);
        laser.transform.localPosition = Vector3.zero;

        // Set the owner to prevent friendly fire
        var mechController = transform.root.GetComponent<PlayerController>()?.mechInstance;
        Debug.Log("MechController: " + mechController);
        if (mechController != null)
        {
            spawnedLaser.SetOwner(mechController.gameObject);
            eyePoint = mechController.eyePoint;
            Debug.Log("here");
        }
    }

    override public void SpecialAttack(PlayerController player, InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SpecialAttack();
        }
    }

    override public void SpecialAttack()
    {
        // TODO: Do the cyclops laser attack logic.
        // Spawn laser pointing in facing direction, originating at eye.
        // Make the head look in the controlling player's joystick direction.
        Debug.Log("Cyclops special attack!");
        animator.SetTrigger("SpecialAttack");

        StartCoroutine(spawnedLaser.ShootLaser_());
    }

}



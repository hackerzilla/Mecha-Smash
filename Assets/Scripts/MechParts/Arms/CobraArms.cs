using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CobraArms : ArmsPart
{
    public GameObject cannonPrefab;
    public Transform armPoint;
    public float delay = 2f;

    private PlayerController playerController;
    private MechController mechController;

    protected override void Awake()
    {
        base.Awake();
        mechController = transform.root.GetComponent<PlayerController>()?.mechInstance;
    }

    override public void BasicAttack(PlayerController player, InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (playerController == null)
        {
            playerController = player;
        }
        if (mechController == null)
        {
            mechController = playerController.mechInstance;
        }
        BasicAttack();
    }

    public override void BasicAttack()
    {
        Debug.Log("Cobra basic attack!");
        animator.SetTrigger("BasicAttack");

        StartCoroutine(FireBothCannons());
    }

    private IEnumerator FireBothCannons()
    {
        if (mechController == null)
        {
            Debug.LogWarning("MechController not found for CobraArms");
            yield break;
        }

        var my_direction = transform.right * Mathf.Sign(mechController.transform.localScale.x);

        var cannon1 = Instantiate(cannonPrefab, armPoint.position, Quaternion.identity);
        Cannon cannon1Script = cannon1.GetComponent<Cannon>();
        cannon1Script.direction = my_direction;
        cannon1Script.SetOwner(mechController.gameObject);

        yield return new WaitForSeconds(delay);

        var cannon2 = Instantiate(cannonPrefab, armPoint.position, Quaternion.identity);
        Cannon cannon2Script = cannon2.GetComponent<Cannon>();
        cannon2Script.direction = my_direction;
        cannon2Script.SetOwner(mechController.gameObject);
    }
}

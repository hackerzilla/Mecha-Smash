using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CobraArms : ArmsPart
{
    public GameObject cannonPrefab;
    public Transform armPoint;
    public float delay = 2f;

    private MechController mechController;

    protected override void Awake()
    {
        base.Awake();
        mechController = transform.root.GetComponent<PlayerController>()?.mechInstance;
    }

    override public void BasicAttack(PlayerController player, InputAction.CallbackContext context)
    {

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
        cannon1.GetComponent<Cannon>().direction = my_direction;
        yield return new WaitForSeconds(delay);
        var cannon2 = Instantiate(cannonPrefab, armPoint.position, Quaternion.identity);
        cannon2.GetComponent<Cannon>().direction = my_direction;
    }
}

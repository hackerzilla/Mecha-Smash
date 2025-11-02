using UnityEngine;
using UnityEngine.InputSystem;

public class StrikerArms : ArmsPart
{
    public override void BasicAttack(PlayerController player, InputAction.CallbackContext context)
    {
        Debug.Log("Striker basic attack!");
        animator.SetTrigger("BasicAttack");
    }
}

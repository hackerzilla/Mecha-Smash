using UnityEngine;
using UnityEngine.InputSystem;


public class StrikerArms : ArmsPart
{
    public override void BasicAttack(PlayerController player, InputAction.CallbackContext context)
    {
        
    }
    public override void BasicAttack()
    {
        animator.SetTrigger("BasicAttack");
    }
}

using UnityEngine;
using UnityEngine.InputSystem;


public class StrikerArms : ArmsPart
{
    public override void BasicAttack(PlayerController player, InputAction.CallbackContext context)
    {
        
    }
    public override void BasicAttack()
    {
        Animator skeletonAnimator = mech.skeletonRig.GetComponent<Animator>();
        if (skeletonAnimator != null)
        {
            skeletonAnimator.SetTrigger("punch");
        }
    }
}

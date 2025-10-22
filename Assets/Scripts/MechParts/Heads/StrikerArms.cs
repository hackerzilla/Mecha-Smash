using UnityEngine;

public class StrikerArms : ArmsPart
{
    public override void BasicAttack()
    {
        Debug.Log("Striker basic attack!");
        animator.SetTrigger("BasicAttack");
    }
}

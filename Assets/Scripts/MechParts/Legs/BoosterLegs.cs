using UnityEngine;

public class BoosterLegs : LegsPart
{
    public override void MovementAbility()
    {
        Debug.Log("Booster movement ability!");
        animator.SetTrigger("MovementAbility");
    }
}

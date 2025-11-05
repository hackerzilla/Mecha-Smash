using UnityEngine;
using UnityEngine.InputSystem;

//By Jae
public class BoosterLegs : LegsPart
{

    protected override void Awake()
    {
        base.Awake();
        AbilityName = "Gas Boosters";
        maxJumps = 2;
    }

    public override void MovementAbility(PlayerController player, InputAction.CallbackContext context)
    {
        return;
    }
}

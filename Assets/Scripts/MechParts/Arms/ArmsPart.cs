using UnityEngine;
using UnityEngine.InputSystem;


abstract public class ArmsPart : MechPart
{
    abstract public void BasicAttack(PlayerController player, InputAction.CallbackContext context);

abstract public class ArmsPart : MechPart
{
    abstract public void BasicAttack();
}

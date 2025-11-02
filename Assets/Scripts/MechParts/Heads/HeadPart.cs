using UnityEngine;
using UnityEngine.InputSystem;

abstract public class HeadPart : MechPart
{
    abstract public void SpecialAttack(PlayerController player, InputAction.CallbackContext context);
}

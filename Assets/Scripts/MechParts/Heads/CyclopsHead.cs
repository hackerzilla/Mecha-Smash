using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CyclopsHead : HeadPart
{
    override public void SpecialAttack(PlayerController player, InputAction.CallbackContext context)
    {
        // TODO: Do the cyclops laser attack logic.
        // Spawn laser pointing in facing direction, originating at eye.
        // Make the head look in the controlling player's joystick direction.
        Debug.Log("Cyclops special attack!");
        animator.SetTrigger("SpecialAttack");
    }   
}

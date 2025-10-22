using UnityEngine;

public class CyclopsHead : HeadPart
{
    override public void SpecialAttack()
    {
        // TODO: Do the cyclops laser attack logic.
        // Spawn laser pointing in facing direction, originating at eye.
        // Make the head look in the controlling player's joystick direction.
        Debug.Log("Cyclops special attack!");
    }   
}

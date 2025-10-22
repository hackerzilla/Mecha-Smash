using UnityEngine;

abstract public class LegsPart : MechPart 
{
    public Transform torsoAttachmentPoint;
    abstract public void MovementAbility();
}

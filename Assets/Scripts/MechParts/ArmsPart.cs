using UnityEngine;

abstract public class ArmsPart : MechPart
{
    public Transform torsoAttachmentPoint;
    abstract public void BasicAttack();
}

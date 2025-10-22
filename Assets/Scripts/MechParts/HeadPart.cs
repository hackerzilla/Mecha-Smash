using UnityEngine;

abstract public class HeadPart : MechPart
{
    public Transform torsoAttachmentPoint;
    abstract public void SpecialAttack();
}

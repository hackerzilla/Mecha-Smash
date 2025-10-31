using UnityEngine;

abstract public class TorsoPart : MechPart
{
    public Transform headAttachmentPoint;
    public Transform armsAttachmentPoint;
    public Transform legsAttachmentPoint;
    abstract public void DefensiveAbility();
}

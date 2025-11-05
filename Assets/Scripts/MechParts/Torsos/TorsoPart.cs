using UnityEngine;
using UnityEngine.InputSystem;

abstract public class TorsoPart : MechPart
{
    [SerializeField] protected float cooldown = 1.0f;
    protected string AbilityName;
    protected float lastUseTime = -1.0f;

    

    public Transform headAttachmentPoint;
    public Transform armsAttachmentPoint;
    public Transform legsAttachmentPoint;

    abstract public void DefensiveAbility(PlayerController player, InputAction.CallbackContext context);

    public virtual bool CanUseAbility()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    protected void StartCooldown()
    {
        lastUseTime = Time.time;
    }
    abstract public void DefensiveAbility();
}

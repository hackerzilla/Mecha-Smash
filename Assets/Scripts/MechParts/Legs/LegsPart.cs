using System.IO.Compression;
using UnityEngine;
using UnityEngine.InputSystem;


abstract public class LegsPart : MechPart
{

    
    [SerializeField]
    protected string AbilityName;
    [SerializeField]
    protected float cooldown = 1.0f;

    public float moveSpeed = 8f;
    public int maxJumps = 1;

    private float lastUseTime = -1.0f;

    abstract public void MovementAbility(PlayerController player, InputAction.CallbackContext context);

    public virtual bool CanUseAbility()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    protected void StartCooldown()
    {
        lastUseTime = Time.time;
    }
}

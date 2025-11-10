using UnityEngine;
using UnityEngine.InputSystem;

public class PlatingTorso : TorsoPart
{

    [SerializeField] private float damageMultiplier = 0.5f;
    [SerializeField] private float duration = 3.0f;

    private MechHealth mechHealth;

    protected override void Awake() 
    {
        base.Awake();
        AbilityName = "Reactive Plating";
    }

    override public void DefensiveAbility() {}
    
    override public void DefensiveAbility(PlayerController player, InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!CanUseAbility())
            {
                Debug.Log(AbilityName + "Cool Down");
                return;
            }

            if (mechHealth == null)
            {
                mechHealth = player.mechInstance.GetComponent<MechHealth>();
                if (mechHealth == null)
                {
                    Debug.LogError("Can't find MechHealth");
                    return;
                }
            }

            Debug.Log(AbilityName + "On");
            animator.SetTrigger("DefensiveAbility");

            mechHealth.ApplyDamageReduction(damageMultiplier, duration);

            StartCooldown();
        }
    }
}

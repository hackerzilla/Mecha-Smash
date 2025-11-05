using UnityEngine;
using UnityEngine.InputSystem;

public class QuantumTorso : TorsoPart
{
    [SerializeField] private float invincibleDuration = 1.5f;

    private MechHealth mechHealth;

    protected override void Awake()
    {
        base.Awake();
        AbilityName = "Quantom Core";
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

            mechHealth.SetInvincible(invincibleDuration);

            StartCooldown();
        }
    }
}

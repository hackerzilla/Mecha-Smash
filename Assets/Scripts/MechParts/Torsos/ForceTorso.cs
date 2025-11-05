using UnityEngine;
using UnityEngine.InputSystem;

public class ForceTorso : TorsoPart
{
    [SerializeField] private float blastRadius = 10f;
    [SerializeField] private float blastForce = 20f;

    [SerializeField] private LayerMask blastLayerMask;

    protected override void Awake()
    {
        base.Awake();
        AbilityName = "Magnetic Repulsion";
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

            Debug.Log(AbilityName + "Perform");
            animator.SetTrigger("DefensiveAbility");

            Vector2 blastOrigin = player.mechInstance.transform.position;
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(blastOrigin, blastRadius, blastLayerMask);

            foreach (var hitCollider in hitColliders)
            {
                Rigidbody2D rb = hitCollider.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = (rb.transform.position - (Vector3)blastOrigin).normalized;
                    rb.AddForce(direction * blastForce, ForceMode2D.Impulse);
                }
            }

            StartCooldown();
        }
    }
    
}

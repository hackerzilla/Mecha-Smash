using UnityEngine;
using UnityEngine.InputSystem;


public class StrikerArms : ArmsPart
{
    [SerializeField] private float damage = 15f;
    [SerializeField] private float stunDuration = 0.5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask enemyLayer;

    public override void BasicAttack(PlayerController player, InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            BasicAttack();
        }
    }
    public override void BasicAttack()
    {
        Debug.Log("Striker basic attack!");
        animator.SetTrigger("BasicAttack");

        Punch();
    }

    private void Punch()
    {
        if(mech == null)
        {
            mech = transform.root.GetComponent<PlayerController>().mechInstance;
        }

        float facingDir = mech.GetFacingDirection();
        Vector2 hitPos = (Vector2)transform.position + new Vector2(facingDir * 1.0f, 0);

        // overlap circle to find enemy
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPos, attackRange, enemyLayer);
        foreach(var hit in hits)
        {
            // except myself
            if(hit.transform.root == mech.transform.root)
            {
                continue;
            }

            MechHealth targetHealth = hit.GetComponentInParent<MechHealth>();
            if(targetHealth != null)
            {
                targetHealth.TakeDamage(damage);

                // give stun
                MechMovement targetMove = targetHealth.GetComponent<MechMovement>();
                if(targetMove != null)
                {
                    targetMove.SetMovementOverride(true, stunDuration);
                    Debug.Log("Enemy Stunned");
                }

                // light knockback
                Rigidbody2D targetRB = targetHealth.GetComponent<Rigidbody2D>();
                if(targetRB != null)
                {
                    targetRB.AddForce(new Vector2(facingDir*5f, 2f), ForceMode2D.Impulse);
                }
            }
        }

    }
}

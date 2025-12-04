using System.Collections;
using UnityEngine;

public class Venom : MonoBehaviour
{
    public VenomHead venomHead;
    private GameObject owner;
    private bool hasHit = false;

    public void SetOwner(GameObject ownerMech)
    {
        owner = ownerMech;
    }

    public void ResetHit()
    {
        hasHit = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (owner == null || hasHit) return;

        // Ignore collisions with the owner mech
        if (collision.gameObject == owner || collision.transform.root.gameObject == owner)
        {
            return;
        }

        // Try to find MechHealth component on the collision or its parent
        MechHealth mechHealth = collision.GetComponentInParent<MechHealth>();
        if (mechHealth != null)
        {
            hasHit = true;  // Prevent re-triggering

            // Deal damage
            StartCoroutine(venomHead.ApplyVenomDoT(mechHealth));
            StartCoroutine(venomHead.HealVenomDoT());

            // Apply knockback - get components from the mech root
            Rigidbody2D rb = mechHealth.GetComponent<Rigidbody2D>();
            MechMovement mechMovement = mechHealth.GetComponent<MechMovement>();

            if (rb != null && mechMovement != null)
            {
                // Disable movement control temporarily
                mechMovement.SetMovementOverride(true);

                // Re-enable movement after 0.2 seconds
                mechMovement.SetMovementOverride(true, 0.2f);
            }
        }
    }
    
    
}

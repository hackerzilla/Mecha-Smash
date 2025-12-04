using System.Collections;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public float maxDistance = 100f;
    public float duration = 0.3f;
    public float damage = 20f;
    public float laserTime = 3;
    public LayerMask hitLayers;

    [Header("Combat")]
    [SerializeField] private float damageInterval = 0.1f;
    [SerializeField] private float knockbackForce = 5f;
    
    [Header("Audio")]
    [SerializeField] protected AudioSource audioSource;

    private LineRenderer lr;
    private GameObject owner;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void SetOwner(GameObject ownerMech)
    {
        owner = ownerMech;
    }

    public IEnumerator ShootLaser_()
    {
        lr.enabled = true;
        if (audioSource != null)
        {
            audioSource.Play();
        }
        float elapsedTime = 0f;

        // Continuously damage while laser is active
        while (elapsedTime < laserTime)
        {
            // Get direction and perform raycast
            var mech = transform.root.GetComponent<PlayerController>().mechInstance;
            var direction = transform.right * Mathf.Sign(mech.transform.localScale.x);

            // Use RaycastAll to get all hits, then filter out owner
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, maxDistance, hitLayers);
            RaycastHit2D validHit = default(RaycastHit2D);

            // Find the first hit that isn't the owner
            foreach (RaycastHit2D hit in hits)
            {
                if (owner != null && (hit.collider.gameObject == owner || hit.transform.root.gameObject == owner))
                {
                    // Skip owner hits
                    continue;
                }

                // Found a valid non-owner hit
                validHit = hit;
                break;
            }

            if (validHit.collider != null)
            {
                // Draw laser to hit point
                Draw2DRay(transform.position, validHit.point);

                // Apply damage and knockback
                ApplyDamageAndKnockback(validHit);
            }
            else
            {
                // Draw laser to max distance
                Draw2DRay(transform.position, transform.position + direction * maxDistance);
            }

            yield return new WaitForSeconds(damageInterval);
            elapsedTime += damageInterval;
        }

        HideLaser();
    }

    private void ApplyDamageAndKnockback(RaycastHit2D hit)
    {
        // Ignore collisions with the owner mech
        if (owner != null && (hit.collider.gameObject == owner || hit.transform.root.gameObject == owner))
        {
            return;
        }

        // Try to find MechHealth component on the collision or its parent
        MechHealth mechHealth = hit.collider.GetComponentInParent<MechHealth>();

        if (mechHealth != null)
        {
            // Deal damage
            mechHealth.TakeDamage(damage);

            // Apply knockback - get components from the mech root
            Rigidbody2D rb = mechHealth.GetComponent<Rigidbody2D>();
            MechMovement mechMovement = mechHealth.GetComponent<MechMovement>();

            if (rb != null && mechMovement != null)
            {
                // Calculate knockback direction and velocity
                Vector2 knockbackDirection = (rb.transform.position - transform.position).normalized;
                Vector2 knockbackVelocity = knockbackDirection * knockbackForce;

                // Disable movement control temporarily
                mechMovement.SetMovementOverride(true);

                // Apply instant velocity change
                rb.linearVelocity = knockbackVelocity;

                // Re-enable movement after 0.2 seconds (with velocity reset)
                mechMovement.SetMovementOverride(true, 0.2f);
            }
        }
        else
        {
            Debug.LogWarning($"No MechHealth found on {hit.collider.gameObject.name} or its parents!");
        }
    }

    private void HideLaser()
    {
        lr.enabled = false;
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    private void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        // Preserve the Z position from the laser transform to ensure proper rendering depth
        Vector3 start = new Vector3(startPos.x, startPos.y, transform.position.z);
        Vector3 end = new Vector3(endPos.x, endPos.y, transform.position.z);

        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}
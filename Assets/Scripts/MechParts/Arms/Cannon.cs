using Unity.VisualScripting;
using UnityEngine;

public class Cannon: MonoBehaviour
{
    public float speed = 6f;
    public float lifetime = 2f;

    public Vector3 direction;

    [SerializeField]
    private CircleCollider2D circle;

    [Header("Combat")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float knockbackForce = 10f;

    private GameObject owner;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public void SetOwner(GameObject ownerMech)
    {
        owner = ownerMech;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore collisions with the owner mech
        if (owner != null && (collision.gameObject == owner || collision.transform.root.gameObject == owner))
        {
            return;
        }

        // Try to find MechHealth component on the collision or its parent
        MechHealth mechHealth = collision.GetComponentInParent<MechHealth>();
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

        // Destroy the cannon on any collision
        Destroy(gameObject);
    }
}
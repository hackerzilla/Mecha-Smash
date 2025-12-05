using UnityEngine;

public class StrikerFist : MonoBehaviour
{
    [SerializeField] private float damage = 25f;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float stunDuration = 0.3f;

    public GameObject owner;
    private bool hasHit = false;

    private void OnDisable()
    {
        hasHit = false;
    }

    public void ResetHit()
    {
        hasHit = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (owner != null && other.transform.root == owner.transform.root)
            return;

        if (hasHit)
            return;

        MechHealth targetHealth = other.GetComponentInParent<MechHealth>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
            hasHit = true;

            // Apply knockback
            MechMovement targetMove = other.GetComponentInParent<MechMovement>();
            Rigidbody2D targetRb = other.GetComponentInParent<Rigidbody2D>();
            if (targetMove != null && targetRb != null)
            {
                targetMove.SetMovementOverride(true, stunDuration);
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                targetRb.linearVelocity = knockbackDir * knockbackForce;
            }
        }
    }
}

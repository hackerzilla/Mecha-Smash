using UnityEngine;

public class StunBlade : MonoBehaviour
{
    [SerializeField] private float damage = 50f;
    private bool hasHIt = false;
    public GameObject owner;

    private void OnDisable() {
        hasHIt = false;
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(owner != null && other.transform.root == owner.transform.root)
        {
            return;
        }

        MechMovement targetMove = other.GetComponentInParent<MechMovement>();

        if(targetMove != null)
        {
            // If keep touched, keep stun
            targetMove.SetMovementOverride(true, 0.1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(owner != null && other.transform.root == owner.transform.root)
        {
            return;
        }

        if (hasHIt)
        {
            return;
        }

        MechHealth targetHealth = other.GetComponentInParent<MechHealth>();
        if(targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
            hasHIt = true;
        }
    }
}

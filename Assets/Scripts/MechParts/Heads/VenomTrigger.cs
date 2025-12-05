using UnityEngine;

public class VenomTrigger : MonoBehaviour
{
    public GameObject owner;
    public VenomHead venomHead;
    private bool hasHit = false;

    public void ResetHit()
    {
        hasHit = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (owner == null || hasHit) return;

        if (collision.transform.root == owner.transform.root) return;

        MechHealth mechHealth = collision.GetComponentInParent<MechHealth>();
        if (mechHealth != null)
        {
            hasHit = true;
            venomHead.OnVenomHit(mechHealth);
        }
    }
}

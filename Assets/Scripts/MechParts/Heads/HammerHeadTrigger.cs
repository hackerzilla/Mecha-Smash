using UnityEngine;

public class HammerHeadTrigger : MonoBehaviour
{
    public GameObject owner;
    public HammerHead hammerHead;

    private bool hasHit = false;

    public void ResetHit()
    {
        hasHit = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (owner == null || hasHit) return;

        // Ignore self
        if (collision.transform.root == owner.transform.root) return;

        hasHit = true;

        // Notify HammerHead of collision (triggers animation, disables charge)
        if (hammerHead != null)
        {
            hammerHead.OnHeadbuttCollision(collision);
        }
    }
}

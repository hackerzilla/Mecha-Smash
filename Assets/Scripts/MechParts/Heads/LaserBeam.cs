using System.Collections;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public float maxDistance = 100f;
    public float duration = 0.3f;
    public float damage = 20f;
    public float laserTime = 3;
    public LayerMask hitLayers;

    private LineRenderer lr;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    public IEnumerator ShootLaser_()
    {
        ShootLaser();
        yield return new WaitForSeconds(laserTime);
        HideLaser();
    }

    private void ShootLaser()
    {
        lr.enabled = true;
        var mech = transform.root.GetComponent<PlayerController>().mechInstance;
        var direction = transform.right * Mathf.Sign(mech.transform.localScale.x);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, hitLayers);
        if (hit.collider != null)
        {
            Draw2DRay(transform.position, hit.point);
        }
        else
        {
            Draw2DRay(transform.position, transform.position + direction * maxDistance);
        }
    }

    private void HideLaser()
    {
        lr.enabled = false;
    }

    private void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
    }
}
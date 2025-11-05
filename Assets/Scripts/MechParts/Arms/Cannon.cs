using UnityEngine;

public class Cannon: MonoBehaviour
{
    public float speed = 6f;
    public float lifetime = 2f;

    public Vector3 direction;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
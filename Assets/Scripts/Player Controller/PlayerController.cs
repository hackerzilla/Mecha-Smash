using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);
    }
}
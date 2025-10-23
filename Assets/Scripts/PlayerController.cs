using NUnit.Framework;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MechController mechPrefab;
    public MechController mechInstance;
    void Start()
    {
        if (mechInstance == null)
        {
            Assert.NotNull(mechPrefab);
            mechInstance = Instantiate(mechPrefab, this.transform);
        }
        Assert.NotNull(mechInstance);
    }

    void Update()
    {
        // TODO replace this code with gamepad unity input system code
        float xInput = Input.GetAxisRaw("Horizontal");
        if (xInput != 0)
        {
            mechInstance.MoveFromInput(xInput);
            mechInstance.legsInstance.animator.SetBool("walking", true);
        }
        else
        {
            mechInstance.legsInstance.animator.SetBool("walking", false);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            mechInstance.Jump();
        }
    }
}

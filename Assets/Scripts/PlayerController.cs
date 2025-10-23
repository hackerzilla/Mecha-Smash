using NUnit.Framework;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MechController mechPrefab;
    public HeadPart headPrefab;
    public TorsoPart torsoPrefab;
    public ArmsPart armsPrefab;
    public LegsPart legsPrefab;
    public MechController mechInstance;
    void Start()
    {
        if (mechInstance == null)
        {
            Assert.NotNull(mechPrefab);
            mechInstance = Instantiate(mechPrefab, this.transform);
            mechInstance.AssembleMechFromPrefabParts(torsoPrefab, headPrefab, armsPrefab, legsPrefab);
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

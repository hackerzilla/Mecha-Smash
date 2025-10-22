using UnityEngine;

/// <summary>
/// Class <c>Mech</c> holds references to the part slots and has all methods related to mech logic.
/// </summary>
public class Mech : MonoBehaviour
{
    public MechPartSlot headPart;
    public MechPartSlot torsoPart;
    public MechPartSlot armsPart;
    public MechPartSlot legsPart;

    private GameObject headParent;
    private GameObject torsoParent;
    private GameObject leftArmParent;
    private GameObject rightArmParent;
    private GameObject legsParent;
    private GameObject rightLegParent;
    private GameObject leftLegParent;

    void Start()
    {
        headParent = GameObject.Find("Head");
        torsoParent = GameObject.Find("Torso");
        leftArmParent = GameObject.Find("LeftArm");
        rightArmParent = GameObject.Find("RightArm");
        legsParent = GameObject.Find("Legs");
        rightLegParent = GameObject.Find("RightLeg");
        leftLegParent = GameObject.Find("LeftLeg");
    }

    void Update()
    {
        
    }
    public void AssembleMech()
    {

    }
}

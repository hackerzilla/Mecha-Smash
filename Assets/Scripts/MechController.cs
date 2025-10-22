using UnityEngine;

/// <summary>
/// Class <c>MechController</c> holds references to the part slots and has all methods related to mech logic.
/// </summary>
public class MechController : MonoBehaviour
{
    public MechPartSlot headPart;
    public MechPartSlot torsoPart;
    public MechPartSlot armsPart;
    public MechPartSlot legsPart;

    private GameObject headParent;
    private GameObject torsoParent;
    private GameObject armsParent;
    private GameObject legsParent;

    void Start()
    {
        headParent = GameObject.Find("Head");
        torsoParent = GameObject.Find("Torso");
        armsParent = GameObject.Find("Arms");
        legsParent = GameObject.Find("Legs");
    }

    void Update()
    {
        
    }
    public void AssembleMech()
    {
        
    }
}

using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Class <c>MechController</c> holds references to the part slots and has all methods related to mech logic.
/// </summary>
public class MechController : MonoBehaviour
{
    public HeadPart headPrefab;
    public TorsoPart torsoPrefab;
    public ArmsPart armsPrefab;
    public LegsPart legsPrefab;

    private HeadPart headInstance;
    private TorsoPart torosInstance;
    private ArmsPart armsInstance;
    private LegsPart legsInstance;
 
    private GameObject headParent;
    private GameObject torsoParent;
    private GameObject armsParent;
    private GameObject legsParent;

    void Start()
    {
        headParent = GameObject.Find("Head");
        if (headParent)
        {
            Debug.Log("Found " + headParent.name);
        }
        torsoParent = GameObject.Find("Torso");
        if (torsoParent)
        {
            Debug.Log("Found " + torsoParent.name);
        }
        armsParent = GameObject.Find("Arms");
        if (armsParent)
        {
            Debug.Log("Found " + armsParent.name);
        }
        legsParent = GameObject.Find("Legs");
        if (legsParent)
        {
            Debug.Log("Found " + legsParent.name);
        }
        AssembleMechParts();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpecialAttack();
        }
    }

    public void MoveFromInput(Vector2 moveInput)
    {
        // TODO: implement movement logic
    }

    public void BasicAttack()
    {
        if (armsInstance)
        {
            armsInstance.BasicAttack();
        }
    }

    public void SpecialAttack()
    {
        if (headInstance)
        {
            headInstance.SpecialAttack();
        }
    }

    public void DefensiveAbility()
    {
        if (torosInstance)
        {
            torosInstance.DefensiveAbility();
        }

    }

    public void MovementAbility()
    {
        if (legsInstance)
        {
            legsInstance.MovementAbility();
        }

    }

    public void AssembleMechParts()
    {
        // TODO: repeat this logic for the torso
        // Assert.AreNotEqual(torsoPrefab, null);

        Assert.AreNotEqual(headPrefab, null);
        headInstance = Instantiate(headPrefab);
        headInstance.transform.SetParent(headParent.transform);
        // note: negate the attachment point's local pos cuz that's now the head's offset from its parent.
        headInstance.transform.SetLocalPositionAndRotation(-headInstance.torsoAttachmentPoint.localPosition, Quaternion.identity);
        // TODO : place the head parent at the torso attachment point

        // TODO: repeat this logic for arms
        // TODO : repeat this logic for legs
        // Assert.AreNotEqual(armsPrefab, null);
        // Assert.AreNotEqual(legsPrefab, null);
    }
}

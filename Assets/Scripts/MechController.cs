using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Class <c>MechController</c> holds references to the part slots and has all methods related to mech logic.
/// </summary>
public class MechController : MonoBehaviour
{
    public HeadPart headPrefab;
    public TorsoPart torsoPrefab;
    public ArmsPart armsPrefab;
    public LegsPart legsPrefab;

    public GameObject headParent;
    public GameObject torsoParent;
    public GameObject armsParent;
    public GameObject legsParent;

    public HeadPart headInstance;
    public TorsoPart torsoInstance;
    public ArmsPart armsInstance;
    public LegsPart legsInstance;
 

    void Start()
    {
        // headParent = GameObject.Find("Head");
        // if (headParent)
        // {
        //     Debug.Log("Found " + headParent.name);
        // }
        // torsoParent = GameObject.Find("Torso");
        // if (torsoParent)
        // {
        //     Debug.Log("Found " + torsoParent.name);
        // }
        // armsParent = GameObject.Find("Arms");
        // if (armsParent)
        // {
        //     Debug.Log("Found " + armsParent.name);
        // }
        // legsParent = GameObject.Find("Legs");
        // if (legsParent)
        // {
        //     Debug.Log("Found " + legsParent.name);
        // }
        AssembleMechParts();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpecialAttack();
        }
    }

    public void MoveFromInput(float xAxisInput)
    {
        Vector3 walkMovement = new Vector3(xAxisInput, 0, 0);
        transform.position += walkMovement * Time.deltaTime;
    }

    public void Jump()
    {
        // TODO implement this
        Debug.Log(name + " jumped!");
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
        if (torsoInstance)
        {
            torsoInstance.DefensiveAbility();
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
        torsoInstance = torsoParent.GetComponentInChildren<TorsoPart>();
        if (torsoInstance == null)
        {
            Assert.AreNotEqual(torsoPrefab, null);
            torsoInstance = Instantiate(torsoPrefab);
            torsoInstance.transform.SetParent(torsoParent.transform);
            torsoInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        }

        headInstance = headParent.GetComponentInChildren<HeadPart>();
        if (headInstance == null)
        {
            Assert.AreNotEqual(headPrefab, null);
            headInstance = Instantiate(headPrefab);
            headInstance.transform.SetParent(headParent.transform);
            // note: negate the attachment point's local pos cuz that's now the head's offset from its parent.
            headInstance.transform.SetLocalPositionAndRotation(-headInstance.torsoAttachmentPoint.localPosition, Quaternion.identity);
            headParent.transform.localPosition = torsoInstance.headAttachmentPoint.localPosition;
        }

        // TODO: repeat this logic for arms
        armsInstance = armsParent.GetComponentInChildren<ArmsPart>();
        if (armsInstance == null)
        {
            Assert.AreNotEqual(armsPrefab, null);
            armsInstance = Instantiate(armsPrefab);
            armsInstance.transform.SetParent(armsParent.transform);
            armsInstance.transform.SetLocalPositionAndRotation(-armsInstance.torsoAttachmentPoint.localPosition, Quaternion.identity);
            armsParent.transform.localPosition = torsoInstance.armsAttachmentPoint.localPosition;
        }

        // TODO : repeat this logic for legs
        legsInstance = legsParent.GetComponentInChildren<LegsPart>();
        if (legsInstance == null)
        {
            Assert.AreNotEqual(legsPrefab, null);
            legsInstance = Instantiate(legsPrefab);
            legsInstance.transform.SetParent(legsParent.transform);
            legsInstance.transform.SetLocalPositionAndRotation(-legsInstance.torsoAttachmentPoint.localPosition, Quaternion.identity);
            legsParent.transform.localPosition = torsoInstance.legsAttachmentPoint.localPosition;
        }
    }
}

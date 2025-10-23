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

    public GameObject torsoParent;    

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
        armsInstance.BasicAttack();
    }

    public void SpecialAttack()
    {
        headInstance.SpecialAttack();
    }

    public void DefensiveAbility()
    {
        torsoInstance.DefensiveAbility();
    }

    public void MovementAbility()
    {
        legsInstance.MovementAbility();
    }

    public void AssembleMechParts()
    {
        Assert.AreNotEqual(torsoPrefab, null);
        torsoInstance = Instantiate(torsoPrefab);
        torsoInstance.transform.SetParent(torsoParent.transform);
        torsoInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);

        Assert.AreNotEqual(headPrefab, null);
        headInstance = Instantiate(headPrefab);
        headInstance.transform.SetParent(torsoInstance.headAttachmentPoint);
        headInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);

        Assert.AreNotEqual(armsPrefab, null);
        armsInstance = Instantiate(armsPrefab);
        armsInstance.transform.SetParent(torsoInstance.armsAttachmentPoint);
        armsInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);

        Assert.AreNotEqual(legsPrefab, null);
        legsInstance = Instantiate(legsPrefab);
        legsInstance.transform.SetParent(torsoInstance.legsAttachmentPoint);
        legsInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
    }
}

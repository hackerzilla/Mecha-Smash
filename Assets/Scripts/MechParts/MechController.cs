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

    // The game object that will basically be the transform that the torso gets attached to.
    public GameObject torsoParent;    

    public HeadPart headInstance;
    public TorsoPart torsoInstance;
    public ArmsPart armsInstance;
    public LegsPart legsInstance;

    void Start()
    {
        // bool ready = torsoPrefab && headPrefab && armsPrefab && legsPrefab;
        // if (ready) AssembleMechParts();
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
        Debug.Log(name + " jumped!");
        // TODO implement this
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

    public void AssembleMechFromPrefabParts(TorsoPart torso, HeadPart head, ArmsPart arms, LegsPart legs)
    {
        torsoPrefab = torso;
        headPrefab = head;
        armsPrefab = arms;
        legsPrefab = legs;
        AssembleMechParts();
    }
    
    public void AssembleMechParts()
    {
        Debug.Log(name + " assembling parts (" + headPrefab.name + ", " + torsoPrefab.name + ", " + armsPrefab.name + ", " + legsPrefab.name + ")");
        AttachTorso();
        AttachHead();
        AttachArms();
        AttachLegs();
    }

    public void SwapPart(MechPart part)
    {
        // TODO implement
        Debug.Log("SwapPart called on " + part.name);
        if (part is HeadPart)
        {
            Debug.Log(part.name + " is a HeadPart");
            headPrefab = (HeadPart)part;
            AttachHead();
        }
        else if (part is TorsoPart)
        {
            Debug.Log(part.name + " is a TorsoPart");
            torsoPrefab = (TorsoPart)part;
            AttachTorso();
            // todo reparent the head, arms and legs to the new torso's attachment points
        }
        else if (part is ArmsPart)
        {
            Debug.Log(part.name + " is a ArmsPart");
        }
        else if (part is LegsPart)
        {
            Debug.Log(part.name + " is a LegsPart");
        }
        else
        {
            Debug.Log(part.name + " is not a valid MechPart subclass!");
        }
    }

    protected void AttachHead()
    {
        if (headInstance != null)
        {
            Destroy(headInstance);
        }
        Assert.AreNotEqual(headPrefab, null);
        headInstance = Instantiate(headPrefab);
        headInstance.transform.SetParent(torsoInstance.headAttachmentPoint);
        headInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
    }
    protected void AttachTorso()
    {
        if (torsoInstance != null)
        {
            Destroy(torsoInstance);
        }
        Assert.AreNotEqual(torsoPrefab, null);
        torsoInstance = Instantiate(torsoPrefab);
        torsoInstance.transform.SetParent(torsoParent.transform);
        torsoInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
    }
    protected void AttachArms()
    {
        if (armsInstance != null)
        {
            Destroy(armsInstance);
        }
        Assert.AreNotEqual(armsPrefab, null);
        armsInstance = Instantiate(armsPrefab);
        armsInstance.transform.SetParent(torsoInstance.armsAttachmentPoint);
        armsInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
    }
    protected void AttachLegs()
    {
        if (legsInstance != null)
        {
            Destroy(legsInstance);
        }
        Assert.AreNotEqual(legsPrefab, null);
        legsInstance = Instantiate(legsPrefab);
        legsInstance.transform.SetParent(torsoInstance.legsAttachmentPoint);
        legsInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
    }
}

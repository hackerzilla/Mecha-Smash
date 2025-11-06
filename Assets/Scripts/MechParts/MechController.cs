using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private MechMovement mechMovement;

    void Awake()
    {
        mechMovement = GetComponent<MechMovement>();
    }


    public void OnMove(Vector2 moveInput)
    {
        if (mechMovement != null)
        {
            mechMovement.SetMoveInput(moveInput);
        }
    }

    public void Jump()
    {
        if (mechMovement != null)
        {
            mechMovement.Jump();
        }
        // TODO: Trigger jump animation
    }

    public void SetMovementOverride(bool isOverriding)
    {
        if (mechMovement != null)
        {
            mechMovement.SetMovementOverride(isOverriding);
        }
    }

    public void SetMovementOverride(bool isOverriding, float duration)
    {
        if (mechMovement != null)
        {
            mechMovement.SetMovementOverride(isOverriding, duration);
        }
    }

    public Rigidbody2D GetRigidbody()
    {
        return mechMovement != null ? mechMovement.GetRigidbody() : null;
    }

    public bool IsGrounded()
    {
        return mechMovement != null && mechMovement.IsGrounded();
    }

    public float GetFacingDirection()
    {
        return mechMovement != null ? mechMovement.GetFacingDirection() : 1f;
    }

    public void BasicAttack(PlayerController player, InputAction.CallbackContext context)
    {
        if (armsInstance != null)
            armsInstance.BasicAttack(player, context);
    }

    public void SpecialAttack(PlayerController player, InputAction.CallbackContext context)
    {
        if (headInstance != null)
            headInstance.SpecialAttack(player, context);
    }

    public void DefensiveAbility(PlayerController player, InputAction.CallbackContext context)
    {
        if (torsoInstance != null)
            torsoInstance.DefensiveAbility(player, context);
    }

    public void MovementAbility(PlayerController player, InputAction.CallbackContext context)
    {
        if(legsInstance != null)
        {
            legsInstance.MovementAbility(player, context);
        }
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
        AttachTorso();
        AttachHead();
        AttachArms();
        AttachLegs();
        ApplyLegStats();
    }

    public void ApplyLegStats()
    {
        if (mechMovement == null)
        {
            Debug.LogError("MechMovement component not found!");
            return;
        }

        if (legsInstance != null)
        {
            mechMovement.SetMoveSpeed(legsInstance.moveSpeed);
            mechMovement.SetMaxJumps(legsInstance.maxJumps);
            Debug.Log($"Legs Stats Applied: {legsInstance.name} (Speed: {legsInstance.moveSpeed}, Jumps: {legsInstance.maxJumps})");
        }
        else
        {
            Debug.LogError("Mech legsInstance is null, cannot apply stats");
            mechMovement.SetMoveSpeed(5f);
            mechMovement.SetMaxJumps(1);
        }
    }

    public void SwapPart(MechPart part)
    {
        if (part is HeadPart)
        {
            headPrefab = (HeadPart)part;
            AttachHead();
        }
        else if (part is TorsoPart)
        {
            torsoPrefab = (TorsoPart)part;
            AttachTorso();
            headInstance.transform.SetParent(torsoInstance.headAttachmentPoint);
            armsInstance.transform.SetParent(torsoInstance.armsAttachmentPoint);
            legsInstance.transform.SetParent(torsoInstance.legsAttachmentPoint);
        }
        else if (part is ArmsPart)
        {
            armsPrefab = (ArmsPart)part;
            AttachArms();
        }
        else if (part is LegsPart)
        {
            legsPrefab = (LegsPart)part;
            AttachLegs();
            ApplyLegStats();
        }
        else
        {
            Debug.LogWarning($"Cannot swap part '{part.name}' - not a valid MechPart subclass");
        }
    }

    protected void AttachHead()
    {
        if (headInstance != null)
        {
            Destroy(headInstance.gameObject);
        }
        Assert.AreNotEqual(headPrefab, null);
        headInstance = Instantiate(headPrefab, torsoInstance.headAttachmentPoint);
        headInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
    }
    protected void AttachTorso()
    {
        if (torsoInstance != null)
        {
            Destroy(torsoInstance.gameObject);
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
            Destroy(armsInstance.gameObject);
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
            Destroy(legsInstance.gameObject);
        }
        Assert.AreNotEqual(legsPrefab, null);
        legsInstance = Instantiate(legsPrefab);
        legsInstance.transform.SetParent(torsoInstance.legsAttachmentPoint);
        legsInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
    }
}

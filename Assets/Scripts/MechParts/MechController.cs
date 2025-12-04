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

    // Skeleton rig reference
    public Transform skeletonRig;

    // Part slot transforms - where part instances get parented
    public Transform headSlot;
    public Transform torsoSlot;
    public Transform armsSlot;
    public Transform legsSlot;

    // Attachment points on skeleton - where part sprites get attached
    public Transform leftHandAttachment;
    public Transform rightHandAttachment;
    public Transform leftCalfAttachment;
    public Transform rightCalfAttachment;
    public Transform rightFootAttachment;
    public Transform leftFootAttachment;
    public Transform eyePoint;

    // Head attachment point on skeleton
    public Transform headAttachment;

    // Torso attachment points on skeleton
    public Transform chestAttachment;
    public Transform coreAttachment;
    public Transform leftShoulderAttachment;
    public Transform rightShoulderAttachment;

    [Header("Visual Settings")]
    [SerializeField] private Material outlineMaterialTemplate;
    public Material torsoMaterialTemplate;
    public Material defaultSpriteMaterial;
    public Material damageFlashMaterial;

    public HeadPart headInstance;
    public TorsoPart torsoInstance;
    public ArmsPart armsInstance;
    public LegsPart legsInstance;

    private MechMovement mechMovement;
    private Animator skeletonAnimator;
    private MechOutlineRenderer outlineRenderer;
    private Color currentOutlineColor = Color.white;

    void Awake()
    {
        mechMovement = GetComponent<MechMovement>();
        outlineRenderer = GetComponent<MechOutlineRenderer>();
        if (skeletonRig != null)
        {
            skeletonAnimator = skeletonRig.GetComponent<Animator>();
        }
    }

    void Start()
    {
        AssembleMechParts();
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
        if (mechMovement != null && mechMovement.TryInitiateJump())
        {
            if (skeletonAnimator != null)
            {
                skeletonAnimator.SetTrigger("jump");
            }
        }
    }

    /// <summary>
    /// Called by MechAnimationEvents bridge when jump animation reaches the launch frame.
    /// </summary>
    public void OnJumpAnimationEvent()
    {
        if (mechMovement != null)
        {
            mechMovement.ApplyJumpForce();
        }
    }

    /// <summary>
    /// Called by MechAnimationEvents bridge when shoot animation triggers right hand gun.
    /// </summary>
    public void OnShootRightHandGunEvent()
    {
        if (armsInstance != null)
        {
            armsInstance.ShootRightHandGun();
        }
    }

    /// <summary>
    /// Called by MechAnimationEvents bridge when shoot animation triggers left hand gun.
    /// </summary>
    public void OnShootLeftHandGunEvent()
    {
        if (armsInstance != null)
        {
            armsInstance.ShootLeftHandGun();
        }
    }

    /// <summary>
    /// Called by MechAnimationEvents bridge when shoot animation completes.
    /// </summary>
    public void OnShootAnimationCompleteEvent()
    {
        if (armsInstance != null)
        {
            armsInstance.OnShootComplete();
        }
    }

    /// <summary>
    /// Called by MechAnimationEvents bridge when headbutt windup ends and charge should begin.
    /// </summary>
    public void OnHeadbuttStartEvent()
    {
        if (headInstance != null)
        {
            headInstance.OnHeadbuttStart();
        }
    }

    /// <summary>
    /// Called by MechAnimationEvents bridge when bite animation reaches the bite frame.
    /// </summary>
    public void OnBiteStartEvent()
    {
        if (headInstance != null)
        {
            headInstance.OnBiteStart();
        }
    }

    /// <summary>
    /// Called by MechAnimationEvents bridge when sword swing hits.
    /// </summary>
    public void OnSwordSwingHitEvent()
    {
        if (armsInstance != null)
        {
            armsInstance.OnSwordSwingHit();
        }
    }

    /// <summary>
    /// Called by MechAnimationEvents bridge when sword swing ends.
    /// </summary>
    public void OnSwordSwingEndEvent()
    {
        if (armsInstance != null)
        {
            armsInstance.OnSwordSwingEnd();
        }
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

    public Animator GetSkeletonAnimator()
    {
        return skeletonAnimator;
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

        // Apply outline to newly assembled parts
        if (outlineRenderer != null)
        {
            outlineRenderer.RefreshSpriteLayers();
            outlineRenderer.SetOutlineColor(currentOutlineColor);
        }
        else if (currentOutlineColor != Color.white)
        {
            ApplyOutlineMaterial();
        }
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
            RefreshOutlineAfterSwap();
        }
        else if (part is TorsoPart)
        {
            torsoPrefab = (TorsoPart)part;
            AttachTorso();
            // Note: Parts are now attached to slots on the skeleton rig,
            // not to the torso's attachment points, so no re-parenting needed
            RefreshOutlineAfterSwap();
        }
        else if (part is ArmsPart)
        {
            armsPrefab = (ArmsPart)part;
            AttachArms();
            RefreshOutlineAfterSwap();
        }
        else if (part is LegsPart)
        {
            legsPrefab = (LegsPart)part;
            AttachLegs();
            ApplyLegStats();
            RefreshOutlineAfterSwap();
        }
        else
        {
            Debug.LogWarning($"Cannot swap part '{part.name}' - not a valid MechPart subclass");
        }
    }

    private void RefreshOutlineAfterSwap()
    {
        if (outlineRenderer != null)
        {
            outlineRenderer.RefreshSpriteLayers();
        }
        else
        {
            ApplyOutlineMaterial();
        }
    }

    protected void AttachHead()
    {
        if (headInstance != null)
        {
            Destroy(headInstance.gameObject);
        }

        // Instantiate part at slot
        headInstance = Instantiate(headPrefab, headSlot);
        headInstance.mech = this;
        headInstance.transform.localScale = Vector3.one;
        headInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);

        // Attach sprites to skeleton rig
        headInstance.AttachSprites(headAttachment);
    }
    protected void AttachTorso()
    {
        if (torsoInstance != null)
        {
            Destroy(torsoInstance.gameObject);
        }

        // Instantiate part at slot
        torsoInstance = Instantiate(torsoPrefab, torsoSlot);
        torsoInstance.mech = this;
        torsoInstance.transform.localScale = Vector3.one;
        torsoInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);

        // Attach sprites to skeleton rig
        torsoInstance.AttachSprites(chestAttachment, coreAttachment, leftShoulderAttachment, rightShoulderAttachment);
    }
    protected void AttachArms()
    {
        if (armsInstance != null)
        {
            Destroy(armsInstance.gameObject);
        }

        // Instantiate part at slot
        armsInstance = Instantiate(armsPrefab, armsSlot);
        armsInstance.mech = this;
        armsInstance.transform.localScale = Vector3.one;
        armsInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);

        // Attach sprites to skeleton rig hand attachment points
        armsInstance.AttachSprites(leftHandAttachment, rightHandAttachment);
    }
    protected void AttachLegs()
    {
        if (legsInstance != null)
        {
            Destroy(legsInstance.gameObject);
        }

        // Instantiate part at slot
        legsInstance = Instantiate(legsPrefab, legsSlot);
        legsInstance.mech = this;
        legsInstance.transform.localScale = Vector3.one;
        legsInstance.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);

        // Attach sprites to skeleton rig foot attachment points
        legsInstance.AttachSprites(leftCalfAttachment, rightCalfAttachment);
    }

    /// <summary>
    /// Sets the outline color for this mech and applies it to all sprite renderers.
    /// </summary>
    public void SetOutlineColor(Color color)
    {
        currentOutlineColor = color;

        // Use new outline renderer if available, otherwise fall back to per-sprite material
        if (outlineRenderer != null)
        {
            outlineRenderer.SetOutlineColor(color);
        }
        else
        {
            ApplyOutlineMaterial();
        }
    }

    /// <summary>
    /// Applies the outline material with the current color to all sprite renderers on the skeleton and parts.
    /// </summary>
    private void ApplyOutlineMaterial()
    {
        if (outlineMaterialTemplate == null || skeletonRig == null)
        {
            Debug.LogWarning("[MechController] Cannot apply outline material - template or skeleton rig is null");
            return;
        }

        // Get all sprite renderers on skeleton and attached parts
        SpriteRenderer[] renderers = skeletonRig.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer renderer in renderers)
        {
            // Create material instance to avoid sharing between players
            Material matInstance = new Material(outlineMaterialTemplate);
            matInstance.SetColor("_OutlineColor", currentOutlineColor);
            renderer.material = matInstance;
        }
    }
}

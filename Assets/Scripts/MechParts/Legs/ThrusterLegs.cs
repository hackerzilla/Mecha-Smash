using UnityEngine;
using UnityEngine.InputSystem;

public class ThrusterLegs : LegsPart
{
    [SerializeField]
    private float thrustForce = 15f;
    [SerializeField]
    private float maxFuel = 3.0f;
    [SerializeField]
    private float rechargeRate = 1.0f;

    [Header("Audio")]
    [SerializeField] private AudioSource jetpackSound;

    private float currentFuel;
    private bool isThrusting;
    private PlayerController playerController;
    private MechAnimationEvents mechAnimEvents;

    protected override void Awake()
    {
        AbilityName = "Jetpack";
        currentFuel = maxFuel;
        maxJumps = 1;
    }

    public override void AttachSprites(Transform leftFootAttachment, Transform rightFootAttachment)
    {
        base.AttachSprites(leftFootAttachment, rightFootAttachment);

        // Hide skeleton's built-in feet
        if (mech != null && mech.skeletonRig != null)
        {
            mechAnimEvents = mech.skeletonRig.GetComponent<MechAnimationEvents>();
            if (mechAnimEvents != null)
            {
                if (mechAnimEvents.leftFootSprite != null)
                    mechAnimEvents.leftFootSprite.SetActive(false);
                if (mechAnimEvents.rightFootSprite != null)
                    mechAnimEvents.rightFootSprite.SetActive(false);
            }
        }
    }

    protected override void OnDestroy()
    {
        // Restore skeleton's feet visibility before cleanup
        if (mechAnimEvents != null)
        {
            if (mechAnimEvents.leftFootSprite != null)
                mechAnimEvents.leftFootSprite.SetActive(true);
            if (mechAnimEvents.rightFootSprite != null)
                mechAnimEvents.rightFootSprite.SetActive(true);
        }

        base.OnDestroy();
    }

    void FixedUpdate()
    {
        if (isThrusting && currentFuel > 0 && playerController != null)
        {
            playerController.GetRigidbody().AddForce(Vector2.up * thrustForce);
            currentFuel -= Time.fixedDeltaTime;
        }
    }
    
    void Update() 
    {
        if (playerController == null) return;
        if (!isThrusting)
        {
            if (currentFuel < maxFuel)
            {
                currentFuel += rechargeRate * Time.deltaTime; 
            }
        }
        else
        {
            if (currentFuel <= 0.0f)
            {
                jetpackSound.Stop();
                isThrusting = false;
            }
        }
    }

    public override void MovementAbility(PlayerController player, InputAction.CallbackContext context)
    {
        if (playerController == null) playerController = player;

        // When click "LegsAbility"
        if (context.performed)
        {
            if (CanUseAbility() && currentFuel > 0)
            {
                isThrusting = true;
                if (jetpackSound != null)
                {
                    jetpackSound.Play();
                }
                StartCooldown();
            }
        }

        // When unclick "LegsAbility"
        if (context.canceled)
        {
            isThrusting = false;
            if (jetpackSound != null)
            {
                jetpackSound.Stop();
            }
        }
    }
    
    public override void MovementAbility()
    {}
}

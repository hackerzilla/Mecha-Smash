using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System.Collections.Generic;

// Handles per-player mech customization in the lobby using Unity Input System.
// Players navigate through part slots (Head/Torso/Arms/Legs) and select from available options.
// Replaces the old PlayerUI.cs and PlayerCustomizer.cs scripts.
public class PlayerCard : MonoBehaviour
{
    [Header("Mech Part Options")]
    public List<HeadPart> headOptions = new List<HeadPart>();
    public List<TorsoPart> torsoOptions = new List<TorsoPart>();
    public List<ArmsPart> armsOptions = new List<ArmsPart>();
    public List<LegsPart> legsOptions = new List<LegsPart>();

    [Header("UI Elements - Part Display")]
    public List<TMP_Text> partNameTexts;
    public List<Image> menuOutlines;
    public float flashingPeriod = 0.25f;

    [Header("Ready System")]
    public TMP_Text readyButtonText;
    public Image readyButtonImage;
    public Color unreadyColor;
    public Color readyColor;
    public Sprite readySprite;
    public Sprite unreadySprite;
    public Sprite readyIndicatorSprite;
    public Sprite unreadyIndicatorSprite;

    [Header("Player-Specific Sprites")]
    public Sprite[] playerNameSprites = new Sprite[4];
    public Sprite[] cardBackgroundSprites = new Sprite[4];

    [Header("Outline Sprites")]
    public Sprite darkOutlineSprite;
    public Sprite highlightedOutlineSprite;

    [Header("Arrow Sprites")]
    public Sprite leftArrowDarkSprite;
    public Sprite leftArrowHighlightedSprite;
    public Sprite rightArrowDarkSprite;
    public Sprite rightArrowHighlightedSprite;
    
    // State
    public PlayerController playerRef;
    private int currentMenuSlot = 0; // Which part slot is currently selected (corresponds to vertical movement)
    private int[] currentPartIndices; // Index into each part list (corresponds to horizontal movement)
    public bool isReady = false;
    private Coroutine activeFlashCoroutine;

    // Input debouncing
    private float lastInputTime = 0f;
    private float inputCooldown = 0.15f; // Prevent accidental double-inputs

    // UI Component References (found at runtime)
    private Image playerNameImage;
    private Image cardBackgroundImage;
    private TMP_Text descriptionText;
    private Image readyTopLeftImage;
    private Image readyBotRightImage;
    private Image[] leftArrows = new Image[4];
    private Image[] rightArrows = new Image[4]; 
    
    
    private const int PART_SLOT_COUNT = 4; // Head, Torso, Arms, Legs

    private void FindUIReferences()
    {
        // Find player-specific UI elements
        Transform playerNameTransform = transform.Find("Player Name Sprite");
        if (playerNameTransform != null)
            playerNameImage = playerNameTransform.GetComponent<Image>();
        else
            Debug.LogWarning("[PlayerCard] Could not find 'Player Name Sprite' GameObject");

        Transform cardBackgroundTransform = transform.Find("Card Background");
        if (cardBackgroundTransform != null)
            cardBackgroundImage = cardBackgroundTransform.GetComponent<Image>();
        else
            Debug.LogWarning("[PlayerCard] Could not find 'Card Background' GameObject");

        // Find description text
        Transform descriptionPanelTransform = transform.Find("Text_Description_Panel");
        if (descriptionPanelTransform != null)
        {
            Transform textTransform = descriptionPanelTransform.Find("Text (TMP)");
            if (textTransform != null)
                descriptionText = textTransform.GetComponent<TMP_Text>();
        }

        // Find ready indicator images
        Transform readyTopLeftTransform = transform.Find("Ready Top Left");
        if (readyTopLeftTransform != null)
            readyTopLeftImage = readyTopLeftTransform.GetComponent<Image>();
        else
            Debug.LogWarning("[PlayerCard] Could not find 'Ready Top Left' GameObject");

        Transform readyBotRightTransform = transform.Find("Ready Bot Right");
        if (readyBotRightTransform != null)
            readyBotRightImage = readyBotRightTransform.GetComponent<Image>();
        else
            Debug.LogWarning("[PlayerCard] Could not find 'Ready Bot Right' GameObject");

        // Find arrows for each part slot
        string[] partSlotNames = { "Helmet", "Torso", "Arms", "Legs" };
        for (int i = 0; i < PART_SLOT_COUNT; i++)
        {
            string outlinePath = $"P1_{partSlotNames[i]}_OutLine";
            string menuPath = $"{outlinePath}/P1_{partSlotNames[i]}_Menu";

            Transform leftArrowTransform = transform.Find($"{menuPath}/LeftArrow");
            if (leftArrowTransform != null)
                leftArrows[i] = leftArrowTransform.GetComponent<Image>();

            // NOTE: The prefab has a typo - "RIghtArrow" (capital I) instead of "RightArrow"
            // Try both spellings to handle the typo
            Transform rightArrowTransform = transform.Find($"{menuPath}/RightArrow");
            if (rightArrowTransform == null)
            {
                rightArrowTransform = transform.Find($"{menuPath}/RIghtArrow");
                if (rightArrowTransform != null)
                {
                    Debug.LogWarning($"[PlayerCard] Found 'RIghtArrow' (with typo) for {partSlotNames[i]}. Please rename to 'RightArrow' in the prefab.");
                }
            }
            if (rightArrowTransform != null)
                rightArrows[i] = rightArrowTransform.GetComponent<Image>();
        }
    }

    private void SetPlayerSpecificUI()
    {
        if (playerRef == null)
        {
            Debug.LogWarning("[PlayerCard] PlayerRef is null, cannot set player-specific UI");
            return;
        }

        // Get playerIndex directly from PlayerInput component (always available when playerRef is set)
        // This avoids race condition with PlayerController.Start() setting playerNumber
        PlayerInput playerInput = playerRef.GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("[PlayerCard] PlayerInput component not found on playerRef");
            return;
        }

        int playerIndex = playerInput.playerIndex; // Already 0-based (Player 1 = 0, Player 2 = 1, etc.)

        // Set player name sprite
        if (playerNameImage != null && playerIndex >= 0 && playerIndex < playerNameSprites.Length)
        {
            if (playerNameSprites[playerIndex] != null)
            {
                playerNameImage.sprite = playerNameSprites[playerIndex];
                Debug.Log($"[PlayerCard] Set player name sprite for Player {playerIndex + 1}");
            }
        }

        // Set card background sprite
        if (cardBackgroundImage != null && playerIndex >= 0 && playerIndex < cardBackgroundSprites.Length)
        {
            if (cardBackgroundSprites[playerIndex] != null)
            {
                cardBackgroundImage.sprite = cardBackgroundSprites[playerIndex];
                Debug.Log($"[PlayerCard] Set card background sprite for Player {playerIndex + 1}");
            }
        }
    }

    void Start()
    {
        FindUIReferences();
        SetPlayerSpecificUI();
        // Initialize to first option for each part type
        currentPartIndices = new int[PART_SLOT_COUNT];
        for (int i = 0; i < PART_SLOT_COUNT; i++)
        {
            currentPartIndices[i] = 0;
        }
        
        UpdateAllPartDisplays();

        // Initialize all outlines and arrows with dark sprites
        for (int i = 0; i < PART_SLOT_COUNT; i++)
        {
            if (menuOutlines.Count > i && menuOutlines[i] != null)
            {
                menuOutlines[i].sprite = darkOutlineSprite;
            }

            if (i < leftArrows.Length && leftArrows[i] != null)
            {
                leftArrows[i].sprite = leftArrowDarkSprite;
            }

            if (i < rightArrows.Length && rightArrows[i] != null)
            {
                rightArrows[i].sprite = rightArrowDarkSprite;
            }
        }

        // Initialize ready indicators to unready state
        if (readyTopLeftImage != null)
            readyTopLeftImage.sprite = unreadyIndicatorSprite;
        if (readyBotRightImage != null)
            readyBotRightImage.sprite = unreadyIndicatorSprite;

        StartFlashing(currentMenuSlot);
        
        if (readyButtonText != null)
        {
            readyButtonText.text = "Unready";
        }
    }
    
    public void SetReady()
    {
        if (isReady)
        {
            readyButtonText.text = "Unready";
            isReady = false;
        }
        else
        {
            readyButtonText.text = "Ready";
            isReady = true;
        }
    }

    public void CheckReady()
    {
        foreach (PlayerController player in LobbyMenuManager.instance.players)
        {
            var ui = player.GetComponent<PlayerController>().playerCard;
    
            if (!ui.isReady)
            {
                return;
            }
    
        }
    
        Debug.Log("All players are ready!");
        LobbyMenuManager.instance.lobbyMenu.SetActive(false); // make the lobby UI invisible
    }
    
    // Called by Unity Input System when Navigate action is triggered (D-pad, left stick, arrow keys).
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!context.performed || isReady) return;
        
        // Cooldown to prevent rapid-fire inputs from held buttons/sticks
        if (Time.time - lastInputTime < inputCooldown) return;
        lastInputTime = Time.time;
        
        Vector2 input = context.ReadValue<Vector2>();
        
        if (Mathf.Abs(input.x) > 0.5f)
        {
            int direction = input.x > 0 ? 1 : -1;
            CyclePart(direction);
        }
        else if (Mathf.Abs(input.y) > 0.2f)
        {
            int direction = input.y > 0 ? -1 : 1; // Up = previous slot, Down = next slot
            SwitchPartSlot(direction);
        }
    }
    
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        ToggleReady();
    }
    
    public MechPart GetSelectedPart()
    {
        switch (currentMenuSlot)
        {
            case 0: return headOptions[currentPartIndices[0]];
            case 1: return torsoOptions[currentPartIndices[1]];
            case 2: return armsOptions[currentPartIndices[2]];
            case 3: return legsOptions[currentPartIndices[3]];
            default: return null;
        }      
    }
    public MechPart GetSelectedHead()
    {
        return (headOptions.Count > 0 && currentPartIndices[0] < headOptions.Count) 
            ? headOptions[currentPartIndices[0]] 
            : null;
    }
    
    public MechPart GetSelectedTorso()
    {
        return (torsoOptions.Count > 0 && currentPartIndices[1] < torsoOptions.Count) 
            ? torsoOptions[currentPartIndices[1]] 
            : null;
    }
    
    public MechPart GetSelectedArms()
    {
        return (armsOptions.Count > 0 && currentPartIndices[2] < armsOptions.Count) 
            ? armsOptions[currentPartIndices[2]] 
            : null;
    }
    
    public MechPart GetSelectedLegs()
    {
        return (legsOptions.Count > 0 && currentPartIndices[3] < legsOptions.Count) 
            ? legsOptions[currentPartIndices[3]] 
            : null;
    }
    
    private int GetPartOptionsCount(int slotIndex)
    {
        switch (slotIndex)
        {
            case 0: return headOptions.Count;
            case 1: return torsoOptions.Count;
            case 2: return armsOptions.Count;
            case 3: return legsOptions.Count;
            default: return 0;
        }
    }
    
    private void CyclePart(int direction)
    {
        int maxIndex = GetPartOptionsCount(currentMenuSlot);
        
        if (maxIndex == 0) return; // No parts available for this slot
        
        currentPartIndices[currentMenuSlot] += direction;
        
        // Wrap around
        if (currentPartIndices[currentMenuSlot] >= maxIndex)
            currentPartIndices[currentMenuSlot] = 0;
        else if (currentPartIndices[currentMenuSlot] < 0)
            currentPartIndices[currentMenuSlot] = maxIndex - 1;
        
        UpdatePartDisplay(currentMenuSlot);
        SwapPartOnMech(currentMenuSlot);
    }
   private void SwapPartOnMech(int slot)
   {
       playerRef.SwapMechPart(GetSelectedPart());
   } 
    
    
    // Switches which part slot is currently being customized (Head -> Torso -> Arms -> Legs)
    private void SwitchPartSlot(int direction)
    {
        StopFlashing();
        
        currentMenuSlot += direction;
        
        // Wrap around
        if (currentMenuSlot >= PART_SLOT_COUNT)
            currentMenuSlot = 0;
        else if (currentMenuSlot < 0)
            currentMenuSlot = PART_SLOT_COUNT - 1;
        
        StartFlashing(currentMenuSlot);
    }
    
    // Toggles ready state. When ready, part selection is locked.
    // Checks if all players are ready to start the game.
    private void ToggleReady()
    {
        isReady = !isReady;

        if (readyButtonText != null)
        {
            readyButtonText.text = isReady ? "Ready" : "Unready";
        }

        // Swap ready button sprite
        if (readyButtonImage != null)
        {
            readyButtonImage.sprite = isReady ? readySprite : unreadySprite;
        }

        // Swap ready indicator sprites
        if (readyTopLeftImage != null)
        {
            readyTopLeftImage.sprite = isReady ? readyIndicatorSprite : unreadyIndicatorSprite;
        }
        if (readyBotRightImage != null)
        {
            readyBotRightImage.sprite = isReady ? readyIndicatorSprite : unreadyIndicatorSprite;
        }

        // Stop part selection when ready
        if (isReady)
        {
            StopFlashing();
        }
        else
        {
            StartFlashing(currentMenuSlot);
        }

        CheckAllPlayersReady();
    }
    
    
    // Checks if all connected players are ready. If so, proceeds to game start.
    private void CheckAllPlayersReady()
    {
        Debug.Assert(LobbyMenuManager.instance != null, "LobbyMenuManager not found!"); 
        foreach (PlayerController player in LobbyMenuManager.instance.players)
        {
            Debug.Assert(player != null, $"Player {player.name} missing PlayerController reference!");
            Debug.Assert(player.playerCard != null, $"Player {player.name} missing playerCard reference!");
            
            if (!player.playerCard.isReady)
            {
                Debug.Log($"Player {player.name} is not ready! Not all players are ready.");
                return;
            }
        }
        
        Debug.Log("All players are ready!");
        
        ApplySelectedPartsToMech(); // does nothing right now, mechs are being assembled in-place
        
        LobbyMenuManager.instance.StartGame();
    }
    
    private void ApplySelectedPartsToMech()
    {
        // DEPRECATED might not even use this function at all, just use SwapMechPart instead
        // TODO 
        // Get the PlayerController this customizer belongs to
        // This assumes the customizer is on the UI, and we need to find the associated player
        // The LobbyMenuManager should maintain this relationship
        
        // For now, this is a placeholder - you'll need to implement the actual part swapping
        // when the player enters the game scene with their selected parts
        Debug.Log($"Player selected: {GetSelectedHead()?.name}, {GetSelectedTorso()?.name}, {GetSelectedArms()?.name}, {GetSelectedLegs()?.name}");
    }
    
    private void UpdatePartDisplay(int slotIndex)
    {
        if (partNameTexts.Count <= slotIndex || partNameTexts[slotIndex] == null)
            return;

        string partName = "None";
        MechPart selectedPart = null;

        switch (slotIndex)
        {
            case 0: // Head
                if (headOptions.Count > 0 && currentPartIndices[0] < headOptions.Count)
                {
                    selectedPart = headOptions[currentPartIndices[0]];
                    partName = selectedPart.name;
                }
                break;
            case 1: // Torso
                if (torsoOptions.Count > 0 && currentPartIndices[1] < torsoOptions.Count)
                {
                    selectedPart = torsoOptions[currentPartIndices[1]];
                    partName = selectedPart.name;
                }
                break;
            case 2: // Arms
                if (armsOptions.Count > 0 && currentPartIndices[2] < armsOptions.Count)
                {
                    selectedPart = armsOptions[currentPartIndices[2]];
                    partName = selectedPart.name;
                }
                break;
            case 3: // Legs
                if (legsOptions.Count > 0 && currentPartIndices[3] < legsOptions.Count)
                {
                    selectedPart = legsOptions[currentPartIndices[3]];
                    partName = selectedPart.name;
                }
                break;
        }

        partNameTexts[slotIndex].text = partName;

        // Update description text to show the selected part's description
        if (descriptionText != null && selectedPart != null)
        {
            descriptionText.text = selectedPart.Description;
        }
    }
    
    private void UpdateAllPartDisplays()
    {
        for (int i = 0; i < PART_SLOT_COUNT; i++)
        {
            UpdatePartDisplay(i);
        }
    }
    
    private void StartFlashing(int slotIndex)
    {
        // Swap outline sprite to highlighted version
        if (menuOutlines.Count > slotIndex && menuOutlines[slotIndex] != null)
        {
            menuOutlines[slotIndex].sprite = highlightedOutlineSprite;
        }

        // Swap arrow sprites to highlighted versions
        if (slotIndex >= 0 && slotIndex < leftArrows.Length)
        {
            if (leftArrows[slotIndex] != null)
                leftArrows[slotIndex].sprite = leftArrowHighlightedSprite;

            if (rightArrows[slotIndex] != null)
                rightArrows[slotIndex].sprite = rightArrowHighlightedSprite;
        }
    }

    private void StopFlashing()
    {
        // Swap outline sprite to dark version
        if (menuOutlines.Count > currentMenuSlot && menuOutlines[currentMenuSlot] != null)
        {
            menuOutlines[currentMenuSlot].sprite = darkOutlineSprite;
        }

        // Swap arrow sprites to dark versions
        if (currentMenuSlot >= 0 && currentMenuSlot < leftArrows.Length)
        {
            if (leftArrows[currentMenuSlot] != null)
                leftArrows[currentMenuSlot].sprite = leftArrowDarkSprite;

            if (rightArrows[currentMenuSlot] != null)
                rightArrows[currentMenuSlot].sprite = rightArrowDarkSprite;
        }
    }
    
    private IEnumerator FlashOutline(Image outline)
    {
        // No longer flashing - just enable the outline as a solid color
        outline.enabled = true;
        yield break;
    }
}
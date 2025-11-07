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
    
    // State 
    public PlayerController playerRef;
    private int currentMenuSlot = 0; // Which part slot is currently selected (corresponds to vertical movement)
    private int[] currentPartIndices; // Index into each part list (corresponds to horizontal movement)
    public bool isReady = false;
    private Coroutine activeFlashCoroutine;
    
    // Input debouncing
    private float lastInputTime = 0f;
    private float inputCooldown = 0.15f; // Prevent accidental double-inputs 
    
    
    private const int PART_SLOT_COUNT = 4; // Head, Torso, Arms, Legs 

    void Start()
    {
        // Initialize to first option for each part type
        currentPartIndices = new int[PART_SLOT_COUNT];
        for (int i = 0; i < PART_SLOT_COUNT; i++)
        {
            currentPartIndices[i] = 0;
        }
        
        UpdateAllPartDisplays();
        
        // Disable all outlines initially
        foreach (var outline in menuOutlines)
        {
            if (outline != null) outline.enabled = false;
        }
        
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
        
        // Stop part selection when ready
        if (isReady)
        {
            StopFlashing();
            // readyButtonImage.color = readyColor;
        }
        else
        {
            // readyButtonImage.color = unreadyColor;
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
        
        switch (slotIndex)
        {
            case 0: // Head
                if (headOptions.Count > 0 && currentPartIndices[0] < headOptions.Count)
                    partName = headOptions[currentPartIndices[0]].name;
                break;
            case 1: // Torso
                if (torsoOptions.Count > 0 && currentPartIndices[1] < torsoOptions.Count)
                    partName = torsoOptions[currentPartIndices[1]].name;
                break;
            case 2: // Arms
                if (armsOptions.Count > 0 && currentPartIndices[2] < armsOptions.Count)
                    partName = armsOptions[currentPartIndices[2]].name;
                break;
            case 3: // Legs
                if (legsOptions.Count > 0 && currentPartIndices[3] < legsOptions.Count)
                    partName = legsOptions[currentPartIndices[3]].name;
                break;
        }
        
        partNameTexts[slotIndex].text = partName;
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
        if (menuOutlines.Count > slotIndex && menuOutlines[slotIndex] != null)
        {
            menuOutlines[slotIndex].enabled = true;
        }
    }
    
    private void StopFlashing()
    {
        if (menuOutlines.Count > currentMenuSlot && menuOutlines[currentMenuSlot] != null)
        {
            menuOutlines[currentMenuSlot].enabled = false;
        }
    }
    
    private IEnumerator FlashOutline(Image outline)
    {
        // No longer flashing - just enable the outline as a solid color
        outline.enabled = true;
        yield break;
    }
}
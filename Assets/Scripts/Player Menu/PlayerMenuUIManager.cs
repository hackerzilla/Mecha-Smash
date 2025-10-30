using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerMenuUIManager : MonoBehaviour
{
    public static PlayerMenuUIManager instance { get; private set; }
    public PlayerInputManager playerInputManager;
    public GameObject playerMenuPrefab;
    public Transform playerMenuParent;
    public bool toggleDebug;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        GameDebug.enabled = toggleDebug;
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += OnPlayerJoined;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        if (playerMenuPrefab == null) return;

        // Create Player UI and Set Player Input to Player Menu
        var playerUI = Instantiate(playerMenuPrefab, playerMenuParent);
        var playerIndex = playerInput.playerIndex + 1;
        playerInput.GetComponent<PlayerUIController>().playerMenuUI = playerUI.GetComponent<PlayerMenuUI>();
        playerUI.name = $"Player {playerIndex} Menu";

        // Set New Name of Player
        playerInput.gameObject.name = $"Player {playerIndex}";

        // Create New Event System
        var eventSystem = new GameObject($"Player {playerIndex} Event System");
        var uiInputModule = eventSystem.AddComponent<InputSystemUIInputModule>();
        uiInputModule.actionsAsset = playerInput.actions;
        eventSystem.transform.SetParent(playerInput.transform, false);

        GameDebug.Log($"Player {playerIndex} joined!");
    }
}

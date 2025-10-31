using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;

public class LobbyMenuManager : MonoBehaviour
{
    // Singleton
    public static LobbyMenuManager instance { get; private set; }
    
    [FormerlySerializedAs("playerUIPrefab")]
    public GameObject playerCardPrefab;
    public PlayerInputManager playerInputManager;
    public GameObject lobbyMenu;
    public List<PlayerController> players;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // singleton pattern
        instance = this;
    }

    private void OnEnable()
    {
        // Subscribe to the Player Input Manager's events.
        playerInputManager.onPlayerJoined += OnPlayerJoined;
        playerInputManager.onPlayerLeft += OnPlayerLeft;
    }

    private void OnDisable()
    {
        // Unsubscribe from the Player Input Manager's events. (avoids memory leaks)
        playerInputManager.onPlayerJoined -= OnPlayerJoined;
        playerInputManager.onPlayerLeft -= OnPlayerLeft;
    }

    // Called every time a player joins via Player Input Manager (a new controller is detected)
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        players.Add(playerInput.GetComponent<PlayerController>());

        if (playerCardPrefab != null)
        {
            PlayerController playerController = playerInput.gameObject.GetComponent<PlayerController>();
            // Create Player UI
            GameObject playerCardObject = Instantiate(playerCardPrefab, lobbyMenu.transform);
            // Attach references 
            PlayerCard playerCard = playerCardObject.GetComponent<PlayerCard>();
            playerController.playerCard = playerCard;
            playerCard.playerRef = playerController;
            // Create Event System and attach to the Player object (not the PlayerCard object!).
            var uiEventSystem = new GameObject($"Player {playerInput.playerIndex} EventSystem");
            var multiplayerEventSystem = uiEventSystem.AddComponent<MultiplayerEventSystem>();
            var uiInputModule = uiEventSystem.AddComponent<InputSystemUIInputModule>();
            uiInputModule.actionsAsset = playerInput.actions;
            multiplayerEventSystem.playerRoot = playerCardObject;
            uiEventSystem.transform.SetParent(playerInput.transform, false);
        }

        Debug.Log($"Player {playerInput.playerIndex} joined!");
    }

    // Called every time a player leaves via Player Input Manager (a new controller is disconnected)
    private void OnPlayerLeft(PlayerInput playerInput)
    {
        players.Remove(playerInput.GetComponent<PlayerController>());

        Debug.Log($"Player {playerInput.playerIndex} left. Total players: {players.Count}");
    }
    
    public void StartGame()
    {
        // make lobby menu invisible
        lobbyMenu.SetActive(false);
        
        // disable the UI input action mapping for all players
        // enable the normal input action mapping for all players
        // any other logic that the player object should run when the game is started goes in PlayerController.OnGameStart
        foreach (PlayerController player in players)
        {
            player.OnGameStart();
        }
    } 
}

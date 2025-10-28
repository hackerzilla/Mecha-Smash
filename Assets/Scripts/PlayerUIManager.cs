using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private Transform playerUIParent;
    public List<GameObject> players;
    public PlayerInputManager playerInputManager;
    public static PlayerUIManager instance { get; private set; }
    public GameObject controllersMenu;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += OnPlayerJoined;
        playerInputManager.onPlayerLeft += OnPlayerLeft;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= OnPlayerJoined;
        playerInputManager.onPlayerLeft -= OnPlayerLeft;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        players.Add(playerInput.gameObject);

        if (playerUIPrefab != null)
        {
            // Create Player UI
            var playerUI = Instantiate(playerUIPrefab, playerUIParent);
            playerInput.GetComponent<PlayerController>().playerUI = playerUI.GetComponent<PlayerUI>();

            // Create Event System
            var uiEventSystem = new GameObject($"Player {playerInput.playerIndex} EventSystem");
            var multiplayerEventSystem = uiEventSystem.AddComponent<MultiplayerEventSystem>();
            var uiInputModule = uiEventSystem.AddComponent<InputSystemUIInputModule>();
            uiInputModule.actionsAsset = playerInput.actions;
            multiplayerEventSystem.playerRoot = playerUI;
            uiEventSystem.transform.SetParent(playerInput.transform, false);
        }

        Debug.Log($"Player {playerInput.playerIndex} joined!");
    }

    private void OnPlayerLeft(PlayerInput playerInput)
    {
        players.Remove(playerInput.gameObject);

        Debug.Log($"Player {playerInput.playerIndex} left. Total players: {players.Count}");
    }
}

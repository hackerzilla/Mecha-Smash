using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance { get; private set; }

    [Header("Restart Settings")]
    [SerializeField] private float restartDelay = 2f;

    [Header("Events")]
    public UnityEvent<int, int> onPlayerCountChanged; // (total, alive)
    public UnityEvent<string> onGameOver;

    private List<PlayerController> trackedPlayers;
    private int totalPlayers = 0;
    private int alivePlayers = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // Singleton pattern
        instance = this;

        // Initialize list and events
        trackedPlayers = new List<PlayerController>();
        onPlayerCountChanged = new UnityEvent<int, int>();
        onGameOver = new UnityEvent<string>();
    }

    private void Start()
    {
        // Subscribe to LobbyMenuManager's player join event
        if (LobbyMenuManager.instance != null)
        {
            LobbyMenuManager.instance.onPlayerJoinedGame.AddListener(OnPlayerJoinedGame);
        }
        else
        {
            Debug.LogError("LobbyMenuManager instance not found!");
        }
    }

    private void OnPlayerJoinedGame(PlayerController player)
    {

        // Add player to tracked list
        trackedPlayers.Add(player);
        totalPlayers++;
        alivePlayers++;

        // Subscribe to this player's death event
        player.onPlayerDeath.AddListener(OnPlayerDeath);

        // Invoke count changed event
        onPlayerCountChanged.Invoke(totalPlayers, alivePlayers);
    }

    public void OnPlayerDeath()
    {

        alivePlayers--;
        Debug.Log($"[GameManager] Player died! Alive players: {alivePlayers}/{totalPlayers}");

        // Invoke count changed event
        onPlayerCountChanged.Invoke(totalPlayers, alivePlayers);

        // Check if only one player remains (the winner!)
        if (alivePlayers <= 1)
        {
            PlayerController winner = GetWinningPlayer();
            string winnerName = "Draw";

            if (winner != null)
            {
                int playerIndex = winner.GetComponent<UnityEngine.InputSystem.PlayerInput>().playerIndex + 1;
                winnerName = $"Player {playerIndex}";
            }

            Debug.Log($"Game Over! Winner: {winnerName}");

            onGameOver.Invoke(winnerName);
        }
    }

    private PlayerController GetWinningPlayer()
    {
        // Find the player whose mech is still alive
        foreach (var player in trackedPlayers)
        {
            if (player.mechInstance != null)
            {
                MechHealth mechHealth = player.mechInstance.GetComponent<MechHealth>();
                if (mechHealth != null && mechHealth.currentHealth > 0)
                {
                    return player;
                }
            }
        }

        return null;
    }

    private IEnumerator RestartGame()
    {
        // Wait for configured delay
        yield return new WaitForSeconds(restartDelay);

        // Reload current scene
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Reloading scene: {currentSceneName}");
        SceneManager.LoadScene(currentSceneName);
    }
}

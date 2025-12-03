using UnityEngine;
using TMPro;               
using UnityEngine.UI;   
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TMP_Text winnerText;
    public Button mainMenuButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if(GameManager.instance != null)
        {
            GameManager.instance.onGameOver.AddListener(OnGameOver);
        }
        
        if(mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }
    }

    void OnGameOver(string winnerName)
    {
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if(winnerText != null)
            {
                winnerText.text = (winnerName == "Draw") ? "Draw Game" : $"{winnerName} win";
            }

            if(mainMenuButton != null)
            {
                mainMenuButton.Select();
            }
        }
    }

    void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        Debug.Log("Returning to Lobby");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

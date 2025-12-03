using UnityEngine;
using TMPro;               
using UnityEngine.UI;   
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TMP_Text winnerText;
    

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

        }
    }

}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ReadySceneController : MonoBehaviour
{
[SerializeField]
    private List<Text> textElements;

    private List<bool> isReady;

    private int totalPlayer = 3;
    private int readyCount = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isReady = new List<bool>();

        foreach (Text txt in textElements)
        {
            isReady.Add(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 'W' key : list[0]
        if (textElements.Count > 0 && !isReady[0] && Input.GetKeyDown(KeyCode.X))
        {
            HandleKeyPress(0);
        }

    // 'I' key : list[1]
        if (textElements.Count > 1 && !isReady[1] && Input.GetKeyDown(KeyCode.Space))
        {
            HandleKeyPress(1);
        }

    // 'UpArrow' key : list[2]
        if (textElements.Count > 2 && !isReady[2] && Input.GetKeyDown(KeyCode.End))
        {
            HandleKeyPress(2);
        }
    }

    void HandleKeyPress(int index)
    {
        // change true of boolean[index]
        isReady[index] = true;

        // change text to red
        if (textElements[index] != null)
        {
            textElements[index].color = Color.red;
        }

        // add readycount
        readyCount++;

        // check readycount reach at totalplayer or not
        if (readyCount >= totalPlayer)
        {
            // Load "Arena" Scene
            SceneManager.LoadScene("Arena");
        }
    }
}

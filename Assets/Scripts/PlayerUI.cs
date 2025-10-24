using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TMP_Text readyButton;
    public bool isReady;

    public void SetReady()
    {
        if (isReady)
        {
            readyButton.text = "Unready";
            isReady = false;
        }
        else
        {
            readyButton.text = "Ready";
            isReady = true;
        }
    }

    public void CheckReady()
    {
        foreach (GameObject player in PlayerUIManager.instance.players)
        {
            var ui = player.GetComponent<PlayerController>().playerUI;

            if (!ui.isReady)
            {
                Debug.Log("Not all players are ready");
                return;
            }

        }

        Debug.Log("All players are ready!");
        PlayerUIManager.instance.controllersMenu.SetActive(false);
    }
}
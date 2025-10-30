using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerMenuUI : MonoBehaviour
{
    [SerializeField] private List<Image> bodyParts;
    [SerializeField] private List<Image> menuOutlines;
    [SerializeField] private List<MenuItem> menuItems;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text readyText;
    public bool isReady;
    private int currentIndex;

    private void Start()
    {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        // Reset Body Party Selection
        HandleBodyPartSwitch(0);

        // Reset Menu Items
        for (int i = 0; i < menuItems.Count; i++)
        {
            menuItems[i].ResetMenu();
        }

        // Set New Name
        playerName.text = gameObject.name;
    }

    public void SetReady()
    {
        if (isReady)
            readyText.text = "Unready";
        else
            readyText.text = "Ready";
        isReady = !isReady;
    }

    public void CheckReady()
    {
        var menuUIs = FindObjectsByType<PlayerMenuUI>(FindObjectsSortMode.None);
        foreach (var menu in menuUIs)
        {
            if (!menu.isReady)
            {
                GameDebug.Log("Not all players are ready");
                return;
            }
        }

        GameDebug.Log("All players are ready!");
        PlayerMenuUIManager.instance.playerMenuParent.gameObject.SetActive(false);
    }

    public void HandleBodyPartSwitch(int direction)
    {
        if (bodyParts == null || bodyParts.Count == 0)
        {
            GameDebug.Error("HandleBodyPartSwitch called but bodyParts list is empty!");
            return;
        }

        // Cycle Through Body Part Index
        currentIndex = (currentIndex + direction) % bodyParts.Count;
        if (currentIndex < 0)
            currentIndex += bodyParts.Count;

        // Highlight Color For Selected Body Part And Set Rest to Black
        foreach (Image bodyPart in bodyParts)
        {
            bodyPart.color = Color.black;
        }
        foreach (Image menuOutline in menuOutlines)
        {
            menuOutline.color = Color.black;
        }
        bodyParts[currentIndex].color = Color.red;
        menuOutlines[currentIndex].color = Color.red;

        GameDebug.Log($"Switched to: {bodyParts[currentIndex].name}");
    }
    
    public void HandleItemSwitch(int direction)
    {
        if (menuItems == null || menuItems.Count == 0)
        {
            GameDebug.Error("HandleItemSwitch called but menu items list is empty!");
            return;
        }

        var currentMenu = menuItems[currentIndex];

        // Cycle Through Current Menu Index
        currentMenu.currentLabelIndex = (currentMenu.currentLabelIndex + direction) % currentMenu.menuItemNames.Length;
        if (currentMenu.currentLabelIndex < 0)
            currentMenu.currentLabelIndex += currentMenu.menuItemNames.Length;

        // Update Menu Text To Selected Text
        currentMenu.menuText.text = currentMenu.menuItemNames[currentMenu.currentLabelIndex];
    }
}

[System.Serializable]
public class MenuItem
{
    public TMP_Text menuText;
    public string[] menuItemNames;
    public int currentLabelIndex = 0;

    public void ResetMenu()
    {
        menuText.text = menuItemNames[0];
    }
}
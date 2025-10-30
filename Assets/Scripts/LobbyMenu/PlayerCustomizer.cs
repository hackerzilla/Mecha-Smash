using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerCustomizer : MonoBehaviour
{

    [Header("Menu Control Key")]
    [SerializeField] private KeyCode leftKey = KeyCode.J;
    [SerializeField] private KeyCode rightKey = KeyCode.L;
    [SerializeField] private KeyCode prevMenuKey = KeyCode.I;
    [SerializeField] private KeyCode nextMenuKey = KeyCode.K;

    [Header("Character Parts")]
    [SerializeField] private List<Image> bodyParts; // [0]=Helmet, [1]=Torso, [2]=Leg

    [Header("UI Elements")]

    [SerializeField] private List<Image> menuColor; // Color indicator of menu
    [SerializeField] private List<Text> menuColorTexts; // Text of parts
    [SerializeField] private List<Image> menuOutlines; // Outline of parts
    [SerializeField] private float flashingPeriod = 0.25f; // Outline flashing period


    private List<Color> colorOptions = new List<Color> { Color.black, Color.red, Color.green, Color.blue };
    private List<string> colorNames = new List<string> { "Black", "RED", "GREEN", "BLUE" };
    
    private int[] currentPartColorIndex; // [0]=Helmet Color, [1]=Torso Color, [2]=Leg Color
    private int currentMenuIndex = 0; // which menu are you looking at.
    private Coroutine activeFlashCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPartColorIndex = new int[bodyParts.Count];

        //Initialize start color of parts as [0], red
        for (int i = 0; i < bodyParts.Count; i++)
        {
            currentPartColorIndex[i] = 0; // 0 = Red
            if (bodyParts.Count > i && bodyParts[i] != null)
            {
                bodyParts[i].color = colorOptions[0];
            }
            if (menuColor.Count > i && menuColor[i] != null)
            {
                menuColor[i].color = colorOptions[0];
            }
            if (menuColorTexts.Count > i && menuColorTexts[i] != null)
            {
                menuColorTexts[i].text = colorNames[0];
            }
        }

        // start with every outlines turned off
        foreach (var border in menuOutlines)
        {
            if (border != null) border.enabled = false;
        }
        
        //when code start, menu of top menu start flash.
        StartFlashing(0);
    }

    // Update is called once per frame
    void Update()
    {
        // check "prevtMenuKey" is clicked.
        if (Input.GetKeyDown(prevMenuKey))
        {
            HandleMenuSwitch(-1);
        }

        // check "NextMenuKey" is clicked.
        if (Input.GetKeyDown(nextMenuKey))
        {
            HandleMenuSwitch(1);
        }

        // check "rightKey" is clicked
        if (Input.GetKeyDown(rightKey))
        {
            ChangeColor(1);
        }

        // check "leftKey" is clicked
        if (Input.GetKeyDown(leftKey))
        {
            ChangeColor(-1);
        }

    }

    // if "NextMenuKey" is clicked, move to next part menu. At last menu, move to frist menu.
    // control flashing
    void HandleMenuSwitch(int direction)
    {

        StopFlashing();
        if (currentMenuIndex == 0 && direction == -1)
        {
            currentMenuIndex = bodyParts.Count - 1;
        }
        else
        {
            currentMenuIndex = (currentMenuIndex + direction) % bodyParts.Count;
        }

        StartFlashing(currentMenuIndex);
    }

    // chagne part color of character as you select at the menu
    void ChangeColor(int direction)
    {
        int colorIndex = currentPartColorIndex[currentMenuIndex];
        colorIndex += direction;

        if (colorIndex >= colorOptions.Count)
        {
            colorIndex = 0;
        }
        else if (colorIndex < 0)
        {
            colorIndex = colorOptions.Count - 1;
        }

        currentPartColorIndex[currentMenuIndex] = colorIndex;
        bodyParts[currentMenuIndex].color = colorOptions[colorIndex];
        menuColor[currentMenuIndex].color = colorOptions[colorIndex];
        menuColorTexts[currentMenuIndex].text = colorNames[colorIndex];
    }
    
    // outline flashing start
    void StartFlashing(int menuIndex)
    {
        if (menuOutlines.Count > menuIndex && menuOutlines[menuIndex] != null)
        {
            activeFlashCoroutine = StartCoroutine(FlashBorder(menuOutlines[menuIndex]));
        }
    }

    // outlne flashing stop
    void StopFlashing()
    {
        if (activeFlashCoroutine != null)
        {
            StopCoroutine(activeFlashCoroutine);
        }
        if (menuOutlines.Count > currentMenuIndex && menuOutlines[currentMenuIndex] != null)
        {
            menuOutlines[currentMenuIndex].enabled = false;
        }
    }

    // coroutine for outline flashing
    IEnumerator FlashBorder(Image border)
    {
        while (true)
        {
            border.enabled = !border.enabled;
            yield return new WaitForSeconds(flashingPeriod);
        }
    }
}

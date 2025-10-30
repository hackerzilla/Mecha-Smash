using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    public PlayerMenuUI playerMenuUI;

    public void HandleNavigation(Vector2 moveInput)
    {
        float vertical = moveInput.y;
        float horizontal = moveInput.x;
        
        bool moveUp = vertical > 0.1f;
        bool moveDown = vertical < -0.1f;
        bool moveLeft = horizontal < -0.1f;
        bool moveRight = horizontal > 0.1f;

        if (moveUp)
            playerMenuUI.HandleBodyPartSwitch(-1);
        else if (moveDown)
            playerMenuUI.HandleBodyPartSwitch(1);
        else if (moveLeft)
            playerMenuUI.HandleItemSwitch(-1);
        else if (moveRight)
            playerMenuUI.HandleItemSwitch(1);
    }

    public void Submit()
    {
        playerMenuUI.SetReady();
        playerMenuUI.CheckReady();
    }
}
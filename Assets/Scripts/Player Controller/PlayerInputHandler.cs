using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerUIController))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerUIController uiController;
    private Vector2 moveInput;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        uiController = GetComponent<PlayerUIController>();
    }

    private void FixedUpdate()
    {
        movement.SetMoveInput(moveInput);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        GameDebug.Log("Move pressed via: " + context.control.device.displayName);

        if (context.performed)
            uiController.HandleNavigation(moveInput);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            movement.Jump();
            GameDebug.Log("Jump pressed via: " + context.control.device.displayName);
        }       
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            uiController.Submit();
            GameDebug.Log($"{gameObject.name} Submit pressed by {GetComponent<PlayerInput>().devices[0].displayName}");
        }
    }
}
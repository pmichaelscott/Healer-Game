using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;

    PlayerController playerController;
    InputActionMap playerActionMap;

    void OnEnable()
    {
        if (inputActions == null)
        {
            Debug.LogError("PlayerInputHandler: No InputActionAsset assigned!", this);
            return;
        }

        playerController = GetComponent<PlayerController>();

        // Get the "Player" action map
        playerActionMap = inputActions.FindActionMap("Player");
        if (playerActionMap == null)
        {
            Debug.LogError("PlayerInputHandler: 'Player' action map not found in InputActionAsset!", this);
            return;
        }

        // Find and subscribe to actions
        var moveAction = playerActionMap.FindAction("Move");
        var jumpAction = playerActionMap.FindAction("Jump");
        var dashAction = playerActionMap.FindAction("Dash");

        if (moveAction != null)
        {
            moveAction.performed += playerController.OnMove;
            moveAction.canceled += playerController.OnMove;
        }
        else
            Debug.LogWarning("PlayerInputHandler: 'Move' action not found!", this);

        if (jumpAction != null)
            jumpAction.started += playerController.OnJump;
        else
            Debug.LogWarning("PlayerInputHandler: 'Jump' action not found!", this);

        if (jumpAction != null)
            jumpAction.canceled += playerController.OnJump;

        if (dashAction != null)
            dashAction.started += playerController.OnDash;
        else
            Debug.LogWarning("PlayerInputHandler: 'Dash' action not found!", this);

        playerActionMap.Enable();
    }

    void OnDisable()
    {
        if (playerActionMap == null) return;

        var moveAction = playerActionMap.FindAction("Move");
        var jumpAction = playerActionMap.FindAction("Jump");
        var dashAction = playerActionMap.FindAction("Dash");

        if (moveAction != null)
        {
            moveAction.performed -= playerController.OnMove;
            moveAction.canceled -= playerController.OnMove;
        }
        if (jumpAction != null)
        {
            jumpAction.started -= playerController.OnJump;
            jumpAction.canceled -= playerController.OnJump;
        }
        if (dashAction != null)
            dashAction.started -= playerController.OnDash;

        playerActionMap.Disable();
    }
}

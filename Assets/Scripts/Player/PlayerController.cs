using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MovementController movementController;
    private UIController uiController;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Get player components
        movementController = GetComponent<MovementController>();
        uiController = GetComponent<UIController>();

        // Initialize components
        movementController.IntializeMovementController();

        // Hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        movementController.HandleMovment();

        if (Input.GetKey(KeyCode.Space))
            movementController.ChargeJump();

        if (Input.GetKeyUp(KeyCode.Space))
            movementController.HandleJump();

        if (Input.GetKeyDown(KeyCode.LeftControl))
            movementController.ToggleCrouch();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            movementController.ToggleSprint();

        movementController.UpdateAnimation();

        uiController.UpdateStaminaBar(movementController.stamina, 100);
    }

    private void FixedUpdate()
    {
        movementController.MovePlayer();
    }

    private void OnCollisionEnter(Collision col)
    {
        movementController.EnterCollision(col);
    }

    private void OnCollisionExit(Collision col)
    {
        movementController.ExitCollision(col);
    }
}
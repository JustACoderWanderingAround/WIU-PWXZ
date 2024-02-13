using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MovementController movementController;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Get player components
        movementController = GetComponent<MovementController>();

        // Initialize components
        movementController.IntializeMovementController();
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
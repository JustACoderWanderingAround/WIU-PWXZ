using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MovementController movementController;
    private UIController uiController;

    // Temporary
    public GameObject metalPipe;
    public Transform itemHoldPoint;

    // Start is called before the first frame update
    void Start()
    {
        // Hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Get player components
        movementController = GetComponent<MovementController>();
        uiController = GetComponent<UIController>();

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

        if (Input.GetKeyDown(KeyCode.F))
        {
            GameObject pipe = Instantiate(metalPipe, itemHoldPoint.position, Quaternion.identity);
            pipe.GetComponent<MetalPipe>().OnUse();
            pipe.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 10f, ForceMode.Impulse);
        }

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
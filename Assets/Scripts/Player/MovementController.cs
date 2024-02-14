using System.Collections;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private MovementData movementData;
    private CapsuleCollider playerCol;
    private Rigidbody playerRB;

    private bool isMoving = false;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private bool isGrounded = true;

    private Vector3 direction;
    public float stamina = 100;
    private float jumpChargeTime = 0;
    private float moveSpeed;
    private bool canJump = true;

    private AnimationController animationController;

    // Start is called before the first frame update
    public void IntializeMovementController()
    {
        //animationManager = AnimationManager.Instance;
        moveSpeed = movementData.walkSpeed;
        playerCol = GetComponent<CapsuleCollider>();
        playerRB = GetComponent<Rigidbody>();
        animationController = AnimationController.Instance;
    }

    public void ToggleSprint()
    {
        if (!isMoving || !isGrounded || isCrouching)
            return;

        isSprinting = !isSprinting;

        if (isSprinting)
        {
            moveSpeed = movementData.sprintSpeed;
        }
        else
        {
            moveSpeed = movementData.walkSpeed;
        }
    }

    public void ToggleCrouch()
    {
        if (!isGrounded)
            return;

        isCrouching = !isCrouching;
        isSprinting = false;

        if (isCrouching)
        {
            moveSpeed = movementData.crouchSpeed;
            playerCol.height = 0.8f;
            playerCol.center = new Vector3(playerCol.center.x, 0.4f, playerCol.center.z);
            playerRB.AddForce(new Vector3(0, 1, 0) * -10f, ForceMode.Impulse);
        }
        else
        {
            moveSpeed = movementData.walkSpeed;
            playerCol.center = new Vector3(playerCol.center.x, 0.9f, playerCol.center.z);
            playerCol.height = 1.78f;
        }
    }

    public void HandleMovment(/*MovementAxisCommand movementCommand*/)
    {
        //if (movementCommand == null)
        //    return;

        // Get input axes
        //float horizontal = movementCommand.HorizontalAxis;
        //float vertical = movementCommand.VerticalAxis;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        isMoving = horizontal != 0 || vertical != 0;

        if (isMoving)
        {
            direction = Camera.main.transform.forward.normalized;

            Vector3 forwardDirection = Vector3.ProjectOnPlane(Camera.main.transform.forward * vertical, Vector3.up);
            Vector3 sideDirection = Vector3.ProjectOnPlane(Camera.main.transform.right * horizontal, Vector3.up);
            direction = (forwardDirection + sideDirection).normalized;

            // If player is sprinting and is moving
            if (isSprinting && playerRB.velocity.magnitude > 0.1f)
                stamina -= movementData.sprintStaminaCost * Time.deltaTime;
        }
        else if (!isMoving && isGrounded)
        {
            playerRB.velocity = Vector3.zero;

            // Disable sprinting if stop moving
            isSprinting = false;

            // Reset move speed
            if (!isCrouching)
                moveSpeed = movementData.walkSpeed;
            else if (isCrouching)
                moveSpeed = movementData.crouchSpeed;
        }

        // Set player to falling if falling
        if (playerRB.velocity.y <= -0.5f)
            animationController.ChangeAnimation(animationController.Falling, 0.1f, 0, 0);

        // If run out of stamina
        if (stamina <= 0f && isSprinting)
        {
            stamina = 0f;
            ToggleSprint();
        }

        // Recharge stamina
        if (!isSprinting && stamina < movementData.maxStamina)
            stamina += movementData.staminaRegenRate * Time.deltaTime;

        // Update facing direction
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetAngle, 0), moveSpeed * Time.deltaTime);
    }

    public void ChargeJump()
    {
        jumpChargeTime += Time.deltaTime;
        if (jumpChargeTime > 1f)
            jumpChargeTime = 1f;
    }

    public void HandleJump()
    {
        if (!canJump || !isGrounded || isCrouching || stamina < movementData.jumpStaminaCost)
            return;

        canJump = false;
        isGrounded = false;

        // Calculate stamina cost
        float totalJumpCost = (movementData.jumpStaminaCost * (jumpChargeTime * movementData.jumpChargeMultiplier)) * 0.75f;
        if (totalJumpCost < movementData.jumpStaminaCost)
            totalJumpCost = movementData.jumpStaminaCost;
        stamina -= totalJumpCost;

        playerRB.velocity = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);
        playerRB.AddForce(transform.up * (movementData.baseJumpForce + jumpChargeTime * movementData.jumpChargeMultiplier), ForceMode.Impulse);

        animationController.ChangeAnimation(animationController.Jump, 0f, 0, 0);

        jumpChargeTime = 0;
    }

    public void UpdateAnimation()
    {
        if (!isGrounded)
            return;

        if (isMoving)
        {
            if (!isSprinting && !isCrouching) animationController.ChangeAnimation(animationController.Walk, 0.15f, 0, 0);
            if (isSprinting && !isCrouching) animationController.ChangeAnimation(animationController.Sprint, 0.15f, 0, 0);
            if (!isSprinting && isCrouching) animationController.ChangeAnimation(animationController.CrouchWalk, 0.15f, 0, 0);
        }
        else
        {
            if (!isCrouching) animationController.ChangeAnimation(animationController.Idle, 0.15f, 0, 0);
            if (isCrouching) animationController.ChangeAnimation(animationController.CrouchIdle, 0.15f, 0, 0);
        }
    }

    public void MovePlayer()
    {
        if (!isMoving)
            return;

        Vector3 force;

        // Adjust drag & force
        if (isGrounded)
            force = direction * moveSpeed * 10f;
        else if (!isGrounded)
            force = direction * moveSpeed * 10f * movementData.airMultiplier;
        else
            force = Vector3.zero;

        // Move player
        playerRB.AddForce(force, ForceMode.Force);
        SpeedControl();
    }

    public void StopPlayer()
    {
        isMoving = false;
        playerRB.velocity = Vector3.zero;
    }

    private void SpeedControl()
    {
        Vector3 currentVel = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);

        if (currentVel.magnitude > moveSpeed)
        {
            Vector3 limitVel = currentVel.normalized * moveSpeed;
            playerRB.velocity = new Vector3(limitVel.x, playerRB.velocity.y, limitVel.z);
        }
    }

    public void EnterCollision(Collision col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Ground")
        {
            isGrounded = true;
            canJump = true;
            playerRB.drag = movementData.groundDrag;
        }
    }

    public void ExitCollision(Collision col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Ground")
        {
            isGrounded = false;
            playerRB.drag = 0;
        }
    }
}
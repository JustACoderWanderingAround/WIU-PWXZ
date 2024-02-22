using System.Collections;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private MovementData movementData;
    private CapsuleCollider playerCol;
    private Rigidbody playerRB;
    private SoundEmitter soundEmitter;
    private FootprintController footprintController;

    private bool isMoving = false;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private bool isGrounded = true;

    private bool inWater => submergence > 0.7f;

    private Vector3 direction;
    public float stamina = 100;
    private float jumpChargeTime = 0;
    private float moveSpeed;
    private bool useStamina = true;
    private bool canJump = true;
    float hazardMult = 0.5f;
    float maxFPTimer = 10f;
    float shitTimer;

    float subOffset = 1.0f;
    float subRange = 2.0f;
    float submergence;
    float buoyancy = 2.25f;
    private float _y = 0f;
    float breathtimer = 100f;
    Vector3 gravity;
    LayerMask waterMask;

    [SerializeField] private AnimationController animationController;

    // Start is called before the first frame update
    public void IntializeMovementController()
    {
        //animationManager = AnimationManager.Instance;
        moveSpeed = movementData.walkSpeed;
        playerCol = GetComponent<CapsuleCollider>();
        playerRB = GetComponent<Rigidbody>();
        soundEmitter = GetComponent<SoundEmitter>();
        footprintController = GetComponent<FootprintController>();
        shitTimer = 0;
        waterMask = LayerMask.NameToLayer("Water");
        gravity = Physics.gravity;
    }

    public void SetUseStamina(bool isUsing)
    {
        useStamina = isUsing;
        if (!useStamina)
            stamina = 100;
    }

    public void ToggleSprint()
    {
        if (!isMoving || !isGrounded || isCrouching)
            return;

        isSprinting = !isSprinting;

        if (isSprinting)
        {
            moveSpeed = movementData.sprintSpeed;
            soundEmitter.SetEmissionRange(movementData.sprintNoiseRange);

            AudioManager.Instance.Stop("Walk");
        }
        else
        {
            moveSpeed = movementData.walkSpeed;
            soundEmitter.SetEmissionRange(movementData.walkNoiseRange);

            AudioManager.Instance.Stop("Sprint");
        }
    }

    public void ToggleCrouch()
    {
        if (!isGrounded || inWater)
            return;

        isCrouching = !isCrouching;
        isSprinting = false;

        if (isCrouching)
        {
            AudioManager.Instance.Stop("Sprint");

            moveSpeed = movementData.crouchSpeed;
            playerCol.height = 0.8f;
            playerCol.center = new Vector3(playerCol.center.x, 0.4f, playerCol.center.z);
            playerRB.AddForce(new Vector3(0, 1, 0) * -10f, ForceMode.Impulse);
            soundEmitter.SetEmissionRange(0);
        }
        else
        {
            moveSpeed = movementData.walkSpeed;
            playerCol.center = new Vector3(playerCol.center.x, 0.9f, playerCol.center.z);
            playerCol.height = 1.78f;
            soundEmitter.SetEmissionRange(movementData.walkNoiseRange);
        }
    }

    public void HandleMovment()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.R))
        {
            _y = 1.75f;
        }
        // move down
        else if (Input.GetKey(KeyCode.F))
        {
            _y = -1.75f;
        }
        else
        {
            _y = 0f;
        }


        if (inWater)
        {
            playerRB.velocity +=
                gravity * ((1f - buoyancy * submergence) * Time.deltaTime);
            breathtimer = Time.deltaTime;
        }


        isMoving = horizontal != 0 || vertical != 0;
        if (isMoving)
        {
            direction = Camera.main.transform.forward.normalized;

            Vector3 forwardDirection = Vector3.ProjectOnPlane(Camera.main.transform.forward * vertical, Vector3.up);
            Vector3 sideDirection = Vector3.ProjectOnPlane(Camera.main.transform.right * horizontal, Vector3.up);
            if (inWater)
            {
                Vector3 upDirection = new Vector3(0, _y, 0);
                direction = (forwardDirection + sideDirection + upDirection).normalized;
            }
            else 
            { 
                direction = (forwardDirection + sideDirection).normalized; 
            }
            

            // If player is walking
            if (!isSprinting && isGrounded)
                AudioManager.Instance.OnlyPlayAfterSoundEnds("Walk");
            // If player is sprinting and is moving
            if (isSprinting && playerRB.velocity.magnitude > 0.1f && useStamina)
            {
                stamina -= movementData.sprintStaminaCost * Time.deltaTime;

                if (isGrounded)
                {
                    AudioManager.Instance.Stop("Walk");
                    AudioManager.Instance.OnlyPlayAfterSoundEnds("Sprint");
                }
            }
            else if (!isSprinting && !isCrouching)
                soundEmitter.SetEmissionRange(movementData.walkNoiseRange);

            soundEmitter.EmitSound(SoundWPosition.SoundType.MOVEMENT);
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

            soundEmitter.SetEmissionRange(0);
        }

        if (!isMoving || !isGrounded)
        {
            AudioManager.Instance.Stop("Walk");
            AudioManager.Instance.Stop("Sprint");
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
        Debug.Log(submergence);
    }

    public void ChargeJump()
    {
        jumpChargeTime += Time.deltaTime;
        if (jumpChargeTime > 1f)
            jumpChargeTime = 1f;
    }

    public void HandleJump()
    {
        if (!canJump || inWater ||!isGrounded || isCrouching || stamina < movementData.jumpStaminaCost)
            return;

        canJump = false;
        isGrounded = false;

        // Calculate stamina cost
        float totalJumpCost = (movementData.jumpStaminaCost * (jumpChargeTime * movementData.jumpChargeMultiplier)) * 0.75f;
        if (totalJumpCost < movementData.jumpStaminaCost)
            totalJumpCost = movementData.jumpStaminaCost;
        if (useStamina)
            stamina -= totalJumpCost;

        playerRB.velocity = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);
        playerRB.AddForce(transform.up * (movementData.baseJumpForce + jumpChargeTime * movementData.jumpChargeMultiplier), ForceMode.Impulse);

        animationController.ChangeAnimation(animationController.Jump, 0f, 0, 0);

        jumpChargeTime = 0;

        AudioManager.Instance.Play("Jump");
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

    public void UpdateFootprints()
    {
        if (!isGrounded)
            return;

        if (isMoving && shitTimer > 0)
            footprintController.CheckFootprint(playerCol);
    }

    public void MovePlayer()
    {
        if (!isMoving)
            return;
        if (shitTimer > 0)
        {
            shitTimer -= Time.deltaTime;
        }
        Vector3 force;



        // Adjust drag & force
        if (isGrounded)
            force = direction * moveSpeed * 10f;
        else if (!isGrounded)
            force = direction * moveSpeed * 10f * movementData.airMultiplier;
        
        else
            force = Vector3.zero;

        // Move player
        playerRB.AddForce(force * hazardMult, ForceMode.Force);
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

    public void CheckSubmergence()
    {
        if (Physics.Raycast(transform.position + transform.up * subOffset,
            -transform.up, out RaycastHit hit) )
        {
            if (hit.collider.tag == "Water")
            {
                submergence = 1.0f - hit.distance / subRange;
            }
            else
            {
                submergence = 1f;
            }
        }

      
    }

    public bool GetInWater()
    {
        return inWater;
    }

    public float GetSubmergence()
    {
        return submergence;
    }

    public void EnterCollision(Collision col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Ground" && !inWater)
        {
            isGrounded = true;
            canJump = true;
            playerRB.drag = movementData.groundDrag;

            soundEmitter.SetEmissionRange(movementData.landNoiseRange);
            soundEmitter.EmitSound(SoundWPosition.SoundType.MOVEMENT);
            AudioManager.Instance.Play("Land");
        }
    }

    public void ExitCollision(Collision col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Ground" && !inWater)
        {
            isGrounded = false;
            playerRB.drag = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hazard") || other.gameObject.CompareTag("Poop"))
            hazardMult = 0.5f;
        if (other.gameObject.CompareTag("Poop"))
        {
            shitTimer = maxFPTimer;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            CheckSubmergence();
            moveSpeed = movementData.swimSpeed;
            playerRB.drag = movementData.waterDrag;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            CheckSubmergence();
            //AudioManager.Instance.Play("Splat");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            moveSpeed = movementData.walkSpeed;
            playerRB.drag = movementData.groundDrag;

        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance = null;
    private MovementController movementController;
    private CameraCapture cameraCapture;
    private UIController uiController;
    private CheckpointController checkpointController;
    private GameObject collidedInteractable;

    public Transform itemHoldPoint;
    public Transform leftHandPoint;
    public Transform rightHandPoint;

    [SerializeField]
    private InventoryManager inventoryManager;

    public bool AddItem(IInventoryItem item)
    {
        return inventoryManager.AddItem(item);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject.transform.parent.gameObject);
            return;
        }
        if (transform.parent == null)
            DontDestroyOnLoad(this);

        // Get player components
        movementController = GetComponent<MovementController>();
        uiController = GetComponent<UIController>();
        cameraCapture = GetComponent<CameraCapture>();
        checkpointController = GetComponent<CheckpointController>();
        cameraCapture.SubscribeOnCapture(OnScreenCapture);

        // Initialize components
        movementController.IntializeMovementController();
        inventoryManager.Init();

        Instance = this;
        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;

        LaserBehaviour.OnSubscribeHit(OnDetectLaserCollision);
    }

    private void OnDisable()
    {
        LaserBehaviour.OnUnsubscribeHit(OnDetectLaserCollision);
    }

    public void OnDetectLaserCollision(Collider col)
    {
        if (col?.gameObject == gameObject)
            CheckpointController.Instance.Load();
    }

    private void OnScreenCapture(GameObject[] gameObjects)
    {
        foreach(GameObject go in gameObjects)
        {
            IInventoryItem item;
            if (go.TryGetComponent(out item))
            {
                inventoryManager.AddItem(item);
                if (item.GetItemIsStackable())
                    Destroy(go);
                else
                    go.SetActive(false);
            }
        }
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

        if (Input.GetKeyDown(KeyCode.L))
            checkpointController.Load();

        if (Input.GetKeyDown(KeyCode.E) && collidedInteractable != null)
            if (collidedInteractable.TryGetComponent(out IInteractable interactable))
                interactable.OnInteract();

        movementController.UpdateAnimation();
        movementController.UpdateFootprints();

        uiController.UpdateStaminaBar(movementController.stamina, 100);

        transform.forward = Camera.main.transform.forward;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    public void SetDontUseStamina(float duration)
    {
        StartCoroutine(DontUseStamina(duration));
    }

    private IEnumerator DontUseStamina(float duration)
    {
        movementController.SetUseStamina(false);
        yield return new WaitForSeconds(duration);
        movementController.SetUseStamina(true);
    }

    private void FixedUpdate()
    {
        movementController.MovePlayer();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (movementController != null)
            movementController.EnterCollision(col);
    }

    private void OnCollisionExit(Collision col)
    {
        movementController.ExitCollision(col);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (checkpointController != null)
            checkpointController.Save(col);

        if (col.CompareTag("Interactable"))
            collidedInteractable = col.gameObject;
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Interactable"))
            collidedInteractable = null;
    }
}
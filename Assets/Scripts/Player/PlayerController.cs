using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance = null;
    private MovementController movementController;
    private CameraCapture cameraCapture;
    private UIController uiController;

    // Temporary
    public GameObject metalPipe;
    public Transform itemHoldPoint;

    [SerializeField]
    private InventoryManager inventoryManager;

    public bool AddItem(IInventoryItem item)
    {
        return inventoryManager.AddItem(item);
    }

    private void Awake()
    {
        Instance = this;
        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Start is called before the first frame update
    void Start()
    { 

        // Get player components
        movementController = GetComponent<MovementController>();
        uiController = GetComponent<UIController>();
        cameraCapture = GetComponent<CameraCapture>();
        cameraCapture.SubscribeOnCapture(OnScreenCapture);

        // Initialize components
        movementController.IntializeMovementController();
        inventoryManager.Init();
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

        if (Input.GetKeyDown(KeyCode.F))
        {
            inventoryManager.UseItem(inventoryManager.items[0].uid);
        }

        if (Input.GetMouseButtonDown(0))
        {
            inventoryManager.UseItem(inventoryManager.items[0].uid);
        }

        movementController.UpdateAnimation();

        uiController.UpdateStaminaBar(movementController.stamina, 100);

        transform.forward = Camera.main.transform.forward;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
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
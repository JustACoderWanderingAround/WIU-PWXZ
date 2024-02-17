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
    private ShopUIController shopController;

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
        if (Instance != null)
        {
            Destroy(gameObject.transform.parent.gameObject);
            return;
        }
        DontDestroyOnLoad(this);

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
        checkpointController = GetComponent<CheckpointController>();
        cameraCapture.SubscribeOnCapture(OnScreenCapture);
        shopController = GetComponent<ShopUIController>();
        


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

        if (Input.GetKeyDown(KeyCode.L))
            checkpointController.Load();

        if (Input.GetKeyDown(KeyCode.E))
            shopController.SetShopCatalogueActive();

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
        movementController.EnterCollision(col);
    }

    private void OnCollisionExit(Collision col)
    {
        movementController.ExitCollision(col);
    }

    private void OnTriggerEnter(Collider other)
    {
        checkpointController.Save(other);
        shopController.SetShopNameActive(other);
    }

    private void OnTriggerExit(Collider other)
    {
        shopController.SetShopNameActive(other);
    }
}
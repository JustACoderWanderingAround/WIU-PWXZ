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
    private ShopUIController shopController;
    private GlobalVolumeController globalVolumeController;
    private CameraController cameraController;

    public Transform itemHoldPoint;
    public Transform leftHandPoint;
    public Transform rightHandPoint;

    private bool isDisabled = false;

    LayerMask waterMask;

    [SerializeField]
    private InventoryManager inventoryManager;

    public void SetIsDisabled(int disabled)
    {
        isDisabled = disabled == 1 ? true : false;
    }

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
        shopController = GetComponent<ShopUIController>();
        globalVolumeController = GetComponent<GlobalVolumeController>();
        cameraController = GetComponent<CameraController>();

        // Initialize components
        movementController.IntializeMovementController();
        inventoryManager.Init();
        cameraController.Initialise();

        Instance = this;
        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;

        LaserBehaviour.OnSubscribeHit(OnDetectLaserCollision);
        waterMask = LayerMask.NameToLayer("Water");
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
        if (isDisabled)
            return;

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
            uiController.SkipThruText();

        if (Input.GetKeyDown(KeyCode.L))
            checkpointController.Load();

        if (Input.GetKeyDown(KeyCode.E))
        {
            shopController.SetShopCatalogueActive();
            if (collidedInteractable != null && collidedInteractable.TryGetComponent(out IInteractable interactable))
            {
                interactable.OnInteract();
            }
        }

        movementController.UpdateAnimation();
        movementController.UpdateFootprints();

        cameraController.ReadMouseAxisCommand(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        cameraController.UpdateTransform();

        uiController.UpdateStaminaBar(movementController.stamina, 100);

        transform.forward = Camera.main.transform.forward;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    public IEnumerator LoadLevel(string nextLevel, Vector3 nextSpawnPos)
    {
        SceneManagement.Instance.LoadScene(nextLevel);

        while (SceneManagement.Instance.isLoading)
        {
            Debug.Log("ISLOADING");
            yield return null;
        }

        transform.position = nextSpawnPos;
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
        if (!movementController.GetInWater())
            movementController.MovePlayer();
        else if (movementController.GetInWater())
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
        if (col.gameObject.CompareTag("Shop"))
        {
            shopController.SetShopNameActive(col);
        }
        if (col.gameObject.CompareTag("Water"))
            globalVolumeController.SetWaterEffect();

        if (col.gameObject.CompareTag("Checkpoint"))
            checkpointController.Save(col);
        if (col.gameObject.CompareTag("ConversationalPartner")) {
            uiController.SetDialogueBoxActive(true);
            uiController.GetConversation(col.GetComponent<ConversationPartner>());
        }
        if (col.CompareTag("Interactable"))
            collidedInteractable = col.gameObject;
    }

    private void OnTriggerExit(Collider col)
    {
        uiController.SetDialogueBoxActive(false);
         if (col.CompareTag("Interactable"))
            collidedInteractable = null;
        if (col.gameObject.CompareTag("Shop"))
            shopController.SetShopNameActive(col);
        if (col.gameObject.CompareTag("Water"))
            globalVolumeController.SetWaterEffect();
    }
}
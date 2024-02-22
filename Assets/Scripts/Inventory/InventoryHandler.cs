using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Created By: Tan Xiang Feng Wayne
/// <summary>
/// This Class Should be used as a intermediate step between ItemManager and BasePickable
/// Handling of on trigger and on enter will be handled here
/// </summary>
public class InventoryHandler : MonoBehaviour
{
    [Header("3D Game Scene")]
    public LayerMask itemLayer;
    public Transform inventoryTransform;
    public Transform activeObjectTransform;

    [Header("UI")]
    public CanvasGroup inventoryCanvas;
    public UIInventory inventoryUIHandler;

    //To show which to render on the ui
    private List<InventorySlot> notActiveSlots = new List<InventorySlot>();

    //Preset 5 inventory slot
    public UIInventorySlot[] activeSlot = new UIInventorySlot[5];
    private UIInventorySlot selectedSlot = null;

    [SerializeField]
    private InventoryManager manager;

    //Set keycodes for the 5 active slot
    private KeyCode[] keyCodes = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
    };

    //5 slots
    private InventorySlot[] inventorySlots = new InventorySlot[5];

    // Start is called before the first frame update
    void Start()
    {
        Initialise();
    }

    // Update: Non Command Pattern (requested)
    void Update()
    {
        // toggle active keycode section
        //Loop through all the possible keycodes
        for (int i = 0; i < keyCodes.Length; i++) {
            if (Input.GetKeyDown(keyCodes[i])) {
                //To support toggle at the same time
                UIInventorySlot newSlot = activeSlot[i];

                //Pressed the second time
                if (selectedSlot != null && selectedSlot == newSlot)
                {
                    //Case 1: Inventory UI Open, Switch Items from bag to active
                    if (inventoryCanvas.gameObject.activeInHierarchy)
                    {
                        //If they confirm want to change
                        if (selectedSlot == newSlot)
                        {
                            selectedSlot.Initialise(inventoryUIHandler.SelectedSlot);
                            inventoryUIHandler.SetSelectedSlot(null);
                            inventoryUIHandler.Clear();
                            StartCoroutine(UIRenderRoutine());
                        }
                    }
                }

                //Select and Unselect mechanic
                selectedSlot?.Highlight(true);

                //Unselect the previous
                if (!selectedSlot?.Slot?.isStackable ?? false && selectedSlot == newSlot)
                {
                    GameObject go = selectedSlot.Slot.goRef;

                    //if its active
                    if (go?.activeInHierarchy ?? false)
                    {
                        //Show the object
                        go.SetActive(false);
                        //Set parent to where it should be placed at
                        go.transform.SetParent(inventoryUIHandler.InventoryTransform);
                    }
                }

                selectedSlot = selectedSlot == newSlot ? null : newSlot;
                selectedSlot?.Highlight();
                break;
            }
        }

        // toggle inventory section
        //Open and Close Inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            //If its not open
            if (!inventoryCanvas.gameObject.activeInHierarchy)
            {
                //Open 
                StartCoroutine(UIRenderRoutine());

                //Unselect the previous
                if (!selectedSlot?.Slot?.isStackable ?? false)
                {
                    GameObject go = selectedSlot.Slot.goRef;

                    if (go?.activeInHierarchy ?? false)
                    {
                        //Show the object
                        go.SetActive(false);
                        //Set parent to where it should be placed at
                        go.transform.SetParent(inventoryTransform);
                    }
                }
                //Set selected slot to null
                selectedSlot?.Highlight(true);
                selectedSlot = null;
            }
            //If its open
            else
            {
                //Clear all the generated object and set active to false
                inventoryUIHandler.Clear();
                inventoryCanvas.gameObject.SetActive(false);
            }
        }

        // using item section
        //Check if the inventory is open, if its open, dont use or toggle since
        //player are switching the items around
        if (!inventoryCanvas.gameObject.activeInHierarchy)
        {
            //Check if the player clicked and they have selected
            if (Input.GetMouseButtonDown(0) && selectedSlot?.Slot != null)
            {
                //Update the manager to know that we have used this item
                manager.UseItem(selectedSlot.Slot.uid);

                //if Item count is 0, set the item ref to null since such item will not exist anymore
                if (selectedSlot.Slot.itemCount <= 0)
                    selectedSlot.Initialise(null);

                //Update the text and image
                selectedSlot.UpdateTransform();
            }

            //If an item is selected (visualisation)
            if (selectedSlot?.Slot != null)
            {
                //Depending if its stackable, if its stackable, NO OBJECT SHOULD BE SHOWN
                //Clear the transform
                if (activeObjectTransform.childCount > (selectedSlot.Slot.isStackable ? 0 : 1))
                {
                    List<Transform> trToR = new List<Transform>();
                    int c = activeObjectTransform.childCount;
                    //Set such as the last one is removed first
                    for (int i = c - 1 ; i >= 0; i--)
                    {
                        trToR.Add(activeObjectTransform.GetChild(i));
                    }
                    trToR.ForEach((t) => t.SetParent(inventoryTransform));
                    trToR.ForEach((t) => SetLayer(t, LayerMask.NameToLayer("Default")));
                }

                GameObject go = selectedSlot.Slot.goRef;
                //not a static func
                if (!selectedSlot.Slot.isStackable && (!(go?.activeInHierarchy ?? true) || selectedSlot.Slot.bFollowHoldPoint))
                {
                    //Show the object
                    go.SetActive(true);
                    SetLayer(go.transform, LayerMask.NameToLayer("ActiveInventory"));
                    //Set parent to where it should be placed at
                    go.transform.SetParent(activeObjectTransform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;

                    Rigidbody rb = go.GetComponent<Rigidbody>(); 
                    if (rb != null)
                        rb.isKinematic = true;
                }
            }
        }
    }

    private bool isActive(InventorySlot compare)
    {
        foreach(UIInventorySlot uis in activeSlot)
        {
            if (uis.Slot == null)
                continue;

            if (uis.Slot == compare)
                return true;
        }
        return false;
    }

    private IEnumerator UIRenderRoutine()
    {
        //Open the canvas
        inventoryCanvas.gameObject.SetActive(true);
        inventoryCanvas.interactable = false;

        //Clear the previous list
        notActiveSlots.Clear();
        //Add in list which can be shown
        manager.items.ForEach((x) =>
        {
            if (!isActive(x))
                notActiveSlots.Add(x);
        });
        
        //Pass in and render it
        inventoryUIHandler.Initialise(notActiveSlots);
        inventoryUIHandler.Render();

        //While its rendering, dont make the canvas interactable
        while (inventoryUIHandler.isRendering)
            yield return null;

        inventoryCanvas.interactable = true;
    }

    public void Initialise()
    {
        manager.Init();
        for (int i = 0; i < 5; i++)
        {
            inventorySlots[i] = null;
        }
        manager.cacheInventoryItemTransform = inventoryTransform;
    }

    public bool AddItemToManager(IInventoryItem item)
    {
        return manager.AddItem(item);
    }

    public bool UseItem(int listIndex)
    {
        //check if the index is correct
        if (listIndex > manager.items.Count || listIndex < 0 || manager.items.Count <= 0)
            return false;

        return manager.UseItem(manager.items[listIndex].uid);
    }

    public bool RemoveItem(int listIndex, bool all = false)
    {
        //check if the index is correct
        if (listIndex > manager.items.Count || listIndex < 0 || manager.items.Count <= 0)
            return false;

        return manager.DiscardItem(manager.items[listIndex].uid, all);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckIfAddItem(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckIfAddItem(other);
    }

    private void CheckIfAddItem(Collider other)
    {
        if ((itemLayer & (1 << other.gameObject.layer)) > 0)
        {
            IInventoryItem item = other.gameObject.GetComponent<IInventoryItem>();
            //Check if theres this component
            if (item != null)
            {
                //Check if can add
                if (manager.AddItem(item))
                {
                    AudioManager.Instance.Play("PickupItem");

                    other.gameObject.SetActive(false);
                    //Set collider to inactive
                    other.enabled = false;
                    other.gameObject.transform.SetParent(inventoryTransform);
                }
            }
        }
    }

    private void SetLayer(Transform tr, int layer)
    {
        tr.gameObject.layer = layer;
        foreach(Transform child in tr.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = layer;
        }
    }
}

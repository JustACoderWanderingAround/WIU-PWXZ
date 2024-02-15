using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Created By: Tan Xiang Feng Wayne
/// <summary>
/// This Class Should be used as a intermediate step between ItemManager and BasePickable
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

    public UIInventorySlot[] activeSlot = new UIInventorySlot[5];
    private UIInventorySlot selectedSlot = null;

    [SerializeField]
    private InventoryManager manager;

    private KeyCode[] keyCodes = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
    };

    //5 slots
    private InventorySlot[] inventorySlots = new InventorySlot[5];

    #region DebugOnly
    // Start is called before the first frame update
    void Start()
    {
        Initialise();
    }

    // Update is called once per frame
    void Update()
    {
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

                selectedSlot?.Highlight(true);
                selectedSlot = selectedSlot == newSlot ? null : newSlot;
                selectedSlot?.Highlight();
                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!inventoryCanvas.gameObject.activeInHierarchy)
            {
                StartCoroutine(UIRenderRoutine());
            }
            else
            {
                inventoryUIHandler.Clear();
                inventoryCanvas.gameObject.SetActive(false);
            }
        }

        if (Input.GetMouseButtonDown(0) && selectedSlot?.Slot != null)
        {
            manager.UseItem(selectedSlot.Slot.uid);

            if (selectedSlot.Slot.itemCount <= 0)
                selectedSlot.Initialise(null);

            selectedSlot.UpdateTransform();
        }
    }
    #endregion

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
        inventoryCanvas.gameObject.SetActive(true);
        inventoryCanvas.interactable = false;

        notActiveSlots.Clear();
        manager.items.ForEach((x) =>
        {
            if (!isActive(x))
                notActiveSlots.Add(x);
        });

        inventoryUIHandler.Initialise(notActiveSlots);
        inventoryUIHandler.Render();

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
                    //If stackable, means static effect
                    if (!item.GetItemIsStackable())
                    {
                        other.gameObject.SetActive(false);
                        //Set collider to inactive
                        other.enabled = false;
                        other.gameObject.transform.SetParent(inventoryTransform);
                    }
                    else
                    {
                        Destroy(other.gameObject);
                    }
                }
            }
        }
    }

    
}

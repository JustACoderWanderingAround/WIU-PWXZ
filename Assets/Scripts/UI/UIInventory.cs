using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//Created by: Tan Xiang Feng Wayne
public class UIInventory : MonoBehaviour
{
    public UIInventorySlot slotPrefab;
    //to be set by Inventory handler 
    public List<InventorySlot> itemsToRender { get; set; }
    //To keep track of the slots spawned to delete
    private List<UIInventorySlot> renderedSlot = new List<UIInventorySlot>();

    private CursorLockMode previousCursorMode;

    //To render accordingly
    private UIInventorySlot hoveringSlot = null;
    private UIInventorySlot selectedSlot = null;

    //To allow inventory handler to get the slot details
    public InventorySlot SelectedSlot { get => selectedSlot?.Slot; }

    //To be initialised within unity
    public CanvasGroup itemCanvas;
    public TMPro.TMP_Text itemText;

    public bool isRendering { get; private set; }

    //Called by Inventory Handler and pass in items that are to be rendered inside the ui
    public void Initialise(List<InventorySlot> _itemsToRender)
    {
        itemsToRender = _itemsToRender;
    }

    //Update per frame
    public void UpdateTransform()
    {
        //if cursor is hovering over something
        if (hoveringSlot)
        {
            //If it is not active, set as active and wait for a frame
            if (!itemCanvas.gameObject.activeInHierarchy)
            {
                //Rebuild the canvas for Content Size Filter to work properly
                itemCanvas.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(itemCanvas.transform as RectTransform);
                return;
            }

            //lerp the alpha to get a smooth transition
            itemCanvas.alpha = Mathf.Lerp(itemCanvas.alpha, 1f, Time.unscaledDeltaTime * 10f);

            //Get Rect Transform of the cursor
            RectTransform rt = itemCanvas.transform as RectTransform;
            //Get Canvas Scaler from the parent (Parent Canvas)
            CanvasScaler scaler = itemCanvas.GetComponentInParent<CanvasScaler>();
            //Get the position of the mouse relative to the screen size and canvas size
            Vector2 mousePosition = new Vector2(Input.mousePosition.x * scaler.referenceResolution.x / Screen.width, Input.mousePosition.y * scaler.referenceResolution.y / Screen.height);
            //Set the offset position based on the itemCanvas size
            //Default TopLeft of canvas follow mousePosition
            Vector2 offsetPosition = new Vector2(rt.sizeDelta.x, -rt.sizeDelta.y) * 0.5f;
            //Make sure it doesnt go out of screen
            if (mousePosition.x + offsetPosition.x * 2f > scaler.referenceResolution.x)
            {
                offsetPosition.x *= -1;
            }
            if (mousePosition.y + offsetPosition.y * 2f < 0f)
            {
                offsetPosition.y *= -1;
            }
            //set the anchored position
            rt.anchoredPosition = mousePosition + offsetPosition;
        }
        else
        {
            //if it is still active, fade out until 0 and set as inactive
            if (itemCanvas.gameObject.activeInHierarchy)
            {
                itemCanvas.alpha = Mathf.Lerp(itemCanvas.alpha, 0f, Time.unscaledDeltaTime * 10f);
                if (itemCanvas.alpha <= 0)
                    itemCanvas.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        UpdateTransform();
    }

    public void SetSelectedSlot(UIInventorySlot slot)
    {
        selectedSlot = slot;
    }
    public void OnPointerEnter(UIInventorySlot slot)
    {
        hoveringSlot = slot;
        itemText.text = slot.Slot.GetString();
        //Rebuild the canvas for Content Size Filter to work properly
        LayoutRebuilder.ForceRebuildLayoutImmediate(itemCanvas.transform as RectTransform);
    }

    public void OnPointerExit(UIInventorySlot slot)
    {
        hoveringSlot = null;
    }

    public void OnPointerClick(UIInventorySlot slot)
    {
        //Unhighlight the previous one and depending if it is they clicked the second time, remove it
        selectedSlot?.Highlight(true);
        selectedSlot = selectedSlot == slot ? null : slot;
        selectedSlot?.Highlight();
    }

    public void Render()
    {
        if (!isRendering)
            StartCoroutine(RenderRoutine());
    }

    private IEnumerator RenderRoutine()
    {
        isRendering = true;
        //Set lockstate to none so we can see cursor
        previousCursorMode = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //Freeze during view
        Time.timeScale = 0f;
        //Create a new gameobject based on the prefab for each object
        foreach (InventorySlot iSlot in itemsToRender)
        {
            UIInventorySlot newUI = Instantiate(slotPrefab, transform);
            //Update the ui
            newUI.Initialise(iSlot);
            //update the list
            renderedSlot.Add(newUI);
            //Handle all the callbacks required
            EventTrigger eventHandler = newUI.GetComponent<EventTrigger>();
            //Enter
            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((ed) => { OnPointerEnter(newUI); });
            eventHandler.triggers.Add(enterEntry);
            //Exit
            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((ed) => { OnPointerExit(newUI); });
            eventHandler.triggers.Add(exitEntry);
            //Click
            EventTrigger.Entry clickEntry = new EventTrigger.Entry();
            clickEntry.eventID = EventTriggerType.PointerClick;
            clickEntry.callback.AddListener((ed) => { OnPointerClick(newUI); });
            eventHandler.triggers.Add(clickEntry);
        }
        isRendering = false;
        yield break;
    }

    public void Clear()
    {
        //Set the lockstate back 
        Cursor.lockState = previousCursorMode;
        Time.timeScale = 1f;
        itemCanvas.alpha = 0;
        itemCanvas.gameObject.SetActive(false);
        if (renderedSlot.Count <= 0) return;

        foreach(UIInventorySlot slot in renderedSlot)
        {
            Destroy(slot.gameObject);
        }
        renderedSlot.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//Created by: Tan Xiang Feng Wayne
public class UIInventory : MonoBehaviour
{
    public UIInventorySlot slotPrefab;
    public List<InventorySlot> itemsToRender { get; set; }
    private List<UIInventorySlot> renderedSlot = new List<UIInventorySlot>();

    private CursorLockMode previousCursorMode;

    private UIInventorySlot hoveringSlot = null;
    private UIInventorySlot selectedSlot = null;

    public InventorySlot SelectedSlot { get => selectedSlot?.Slot; }

    public CanvasGroup itemCanvas;
    public TMPro.TMP_Text itemText;

    public bool isRendering { get; private set; }

    public void Initialise(List<InventorySlot> _itemsToRender)
    {
        itemsToRender = _itemsToRender;
    }

    public void UpdateTransform()
    {
        if (hoveringSlot)
        {
            if (!itemCanvas.gameObject.activeInHierarchy)
            {
                itemCanvas.gameObject.SetActive(true);
                return;
            }

            itemCanvas.alpha = Mathf.Lerp(itemCanvas.alpha, 1f, Time.unscaledDeltaTime * 10f);

            RectTransform rt = itemCanvas.transform as RectTransform;
            CanvasScaler scaler = itemCanvas.GetComponentInParent<CanvasScaler>();
            rt.anchoredPosition = new Vector2(Input.mousePosition.x * scaler.referenceResolution.x / Screen.width, Input.mousePosition.y * scaler.referenceResolution.y / Screen.height) - new Vector2(-rt.sizeDelta.x, rt.sizeDelta.y) * 0.5f;

            LayoutRebuilder.ForceRebuildLayoutImmediate(itemCanvas.transform as RectTransform);
        }
        else
        {
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
    }

    public void OnPointerExit(UIInventorySlot slot)
    {
        hoveringSlot = null;
    }

    public void OnPointerClick(UIInventorySlot slot)
    {
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
        previousCursorMode = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

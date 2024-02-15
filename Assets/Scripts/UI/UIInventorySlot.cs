using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//Created by: Tan Xiang Feng Wayne
public class UIInventorySlot : MonoBehaviour
{
    private InventorySlot slot;
    public InventorySlot Slot { get => slot; }
    [SerializeField]
    private Image baseImage, itemImage, overlayImage;
    [SerializeField]
    private TMP_Text countText;

    Color originalOverlayColor;

    private void Awake()
    {
        originalOverlayColor = overlayImage.color;
    }

    public void Initialise(InventorySlot _slot)
    {
        slot = _slot;

        countText.text = (slot?.isStackable ?? false) ? "" + (slot?.itemCount ?? 0) : "";
        itemImage.sprite = slot?.itemDisplayImage;
    }

    public void UpdateTransform()
    {
        countText.text = (slot?.isStackable ?? false) ? "" + (slot?.itemCount ?? 0) : "";
        itemImage.sprite = slot?.itemDisplayImage;
    }

    public void Highlight(bool remove = false)
    {
        if (overlayImage)
            overlayImage.color = remove ? originalOverlayColor : Color.yellow;
    }
}

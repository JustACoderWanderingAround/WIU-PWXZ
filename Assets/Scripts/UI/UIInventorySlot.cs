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
    private static Sprite defaultItemSprite = null;

    private void Awake()
    {
        originalOverlayColor = overlayImage.color;
        if (defaultItemSprite == null)
        {
            //Create an copy of the default texture and create a new sprite
            //Create a new texture 2d with exact width and height
            Texture2D defaultTexture = new Texture2D(itemImage.sprite.texture.width, itemImage.sprite.texture.height);
            //Copy the pixel over
            defaultTexture.SetPixels(itemImage.sprite.texture.GetPixels());
            //Apply it
            defaultTexture.Apply();
            defaultItemSprite = Sprite.Create(defaultTexture, new Rect(0, 0, defaultTexture.width, defaultTexture.height),
                        new Vector2(defaultTexture.width * 0.5f, defaultTexture.height * 0.5f));
        }
    }

    public void Initialise(InventorySlot _slot)
    {
        slot = _slot;

        UpdateTransform();
    }

    public void UpdateTransform()
    {
        //Set text accordingly depending if it stackable
        countText.text = (slot?.isStackable ?? false) ? "" + (slot?.itemCount ?? 0) : "";
        itemImage.overrideSprite = slot?.itemDisplayImage;
    }

    public void Highlight(bool remove = false)
    {
        if (overlayImage)
            overlayImage.color = remove ? originalOverlayColor : Color.yellow;
    }
}

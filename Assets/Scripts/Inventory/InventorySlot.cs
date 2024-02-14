using System;
using System.Collections;
using UnityEngine;

//Created by: Tan Xiang Feng Wayne
[Serializable]
public class InventorySlot
{
    public Sprite itemDisplayImage;
    public int uid;
    public string itemName;
    public string itemDescription;
    public bool isStackable;
    public int itemCount;
    public Action itemEffect;

    private const string folderPath = "Inventory";

    public InventorySlot(int _uid, string _itemName, string _itemDescription, 
        Action _itemEffect, Sprite _itemDisplayImage, bool _isStackable = false, int _itemCount = 1)
    {
        uid = _uid;
        itemName = _itemName;
        itemDescription = _itemDescription;
        itemEffect = _itemEffect;
        isStackable = _isStackable;
        itemCount = _itemCount;
        itemDisplayImage = _itemDisplayImage;

        //Try and Load Image from resources if forgotten to add
        if (itemDisplayImage == null)
            itemDisplayImage = Resources.Load<Sprite>(System.IO.Path.Combine(folderPath, itemName));
    }
}

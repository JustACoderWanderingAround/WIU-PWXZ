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
    public bool isConsumable;
    public int itemCount;
    public Action itemEffect;

    private const string folderPath = "Inventory";
    public GameObject goRef { get; private set; }

    public InventorySlot(int _uid, string _itemName, string _itemDescription, 
        Action _itemEffect, Sprite _itemDisplayImage, GameObject obj, bool _isStackable = false, bool _isConsumable = false, int _itemCount = 1)
    {
        uid = _uid;
        itemName = _itemName;
        itemDescription = _itemDescription;
        itemEffect = _itemEffect;
        isStackable = _isStackable;
        itemCount = _itemCount;
        itemDisplayImage = _itemDisplayImage;
        isConsumable = _isConsumable;
        goRef = obj;

        //Try and Load Image from resources if forgotten to add
        if (itemDisplayImage == null)
            itemDisplayImage = Resources.Load<Sprite>(System.IO.Path.Combine(folderPath, itemName));
    }

    public string GetString()
    {
        return $"<b>Name:</b> {itemName}\n<b>Description: </b>{(itemDescription == string.Empty ? "NULL" : itemDescription)}\n" + (isStackable ? $"<b>Item Count:</b> {itemCount}" : "") + $"\n\n<b>IsStackable: </b>{isStackable}\n<b>IsConsumable: </b>{isConsumable}";
    }
}

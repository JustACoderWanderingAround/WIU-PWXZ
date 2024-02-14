using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created By: Tan Xiang Feng Wayne
[CreateAssetMenu(fileName = "InventoryManager", menuName = "Inventory/Manager")]
public class InventoryManager : ScriptableObject
{
    [Header("Settings")]
    public int maxItemsPerSlot = 5;
    public int maxSlots = 5; //this should be depending on the ui

    //So It Could be converted to json later on
    public List<InventorySlot> items { get; private set; }

    public string itemsJSON = string.Empty;

    public void Init()
    {
        items = new List<InventorySlot>();
    }

    /// <summary>
    /// Add Item to the inventory lists
    /// </summary>
    /// <param name="newItem">itemToBeProcessed</param>
    /// <returns>true if managed to add item, false if the max slot</returns>
    public bool AddItem(IInventoryItem newItem)
    {
        //Try and find if there is such item in the inventory already
        InventorySlot itemSlot = items.Find((item) => item.itemName == newItem.GetItemName());

        //If its a new and unique item or item has already exceeded max count
        if ((itemSlot == null) || (itemSlot.itemCount >= (itemSlot.isStackable ? maxItemsPerSlot : 1)))
        {
            //Cannot create new slot
            if (items.Count > maxSlots)
                return false;

            //Create a unique ID
            int uniqueID;
            do
            {
                uniqueID = Random.Range(0, int.MaxValue);
            }
            while (items.Find((item) => item.uid == uniqueID) != null);

            //Create a new slot
            itemSlot = new InventorySlot(uniqueID, newItem.GetItemName(), newItem.GetItemDescription(),
                newItem.GetItemEffect(), newItem.GetItemDisplaySprite(), newItem.GetItemIsStackable(), 0);

            //Add it to the list
            items.Add(itemSlot);
        }
        
        //Add the item count
        itemSlot.itemCount++;

        itemsJSON = ToJSON();

        return true;
    }

    public bool DiscardItem(int uid, bool removeAll = false)
    {
        //Find if the items contains the slot with the uid
        InventorySlot slot = items.Find((item) => item.uid == uid);

        //No Such Items in the slot
        if (slot == null)
            return false;

        //Decrease the item count
        slot.itemCount--;

        //Remove depending on
        if (removeAll || slot.itemCount <= 0)
            items.Remove(slot);

        return true;
    }

    public bool UseItem(int uid)
    {
        //Find if the items contains the slot with the uid
        InventorySlot slot = items.Find((item) => item.uid == uid);

        //No Such Items in the slot
        if (slot == null || slot.itemCount <= 0)
            return false;

        //Decrease the item count
        slot.itemCount--;

        if (slot.itemCount <= 0)
            items.Remove(slot);

        //Invoke the item effect
        slot.itemEffect.Invoke();
        return true;
    }

    public string ToJSON()
    {
        return JsonUtility.ToJson(new SerializableList<InventorySlot>(items));
    }
}

[System.Serializable]
public class SerializableList<Type>
{
    public List<Type> list = new List<Type>();

    public SerializableList(List<Type> _list)
    {
        list = _list;
    }
}

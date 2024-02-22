using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created By: Tan Xiang Feng Wayne
[CreateAssetMenu(fileName = "InventoryManager", menuName = "Inventory/Manager")]
public class InventoryManager : ScriptableObject
{
    [Header("Settings")]
    public int maxItemsPerSlot = 30;
    public int maxSlots = 5; //this should be depending on the ui

    //So It Could be converted to json later on
    public List<InventorySlot> items { get; private set; }

    public string itemsJSON = string.Empty;

    public Transform cacheInventoryItemTransform { get; set; }

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
        //and find the first instance that can add the item
        //If item is not stackable, just set as null so we can create later on
        InventorySlot itemSlot = newItem.GetItemIsStackable() ? 
            items.Find((item) => item.itemName == newItem.GetItemName() && (item.itemCount < maxItemsPerSlot)) : null;

        //If its a new and unique item or item has already exceeded max count
        if (itemSlot == null)
        {
            //Cannot create new slot
            if (items.Count >= maxSlots)
                return false;

            //Create a unique ID
            int uniqueID;
            //Loop through to make sure its a totally unique id
            do
            {
                uniqueID = Random.Range(0, int.MaxValue);
            }
            while (items.Find((item) => item.uid == uniqueID) != null);

            //Create a new slot
            itemSlot = new InventorySlot(uniqueID, newItem.GetItemName(), newItem.GetItemDescription(),
                newItem.GetItemEffect(), newItem.GetItemDisplaySprite(), newItem.GetGameObject(), newItem.GetFollowHoldPoint(),
                newItem.GetItemIsStackable(), newItem.GetItemIsConsumable(), 0);

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

        if (slot.isConsumable)
        {
            //Decrease the item count
            slot.itemCount--;

            if (slot.itemCount <= 0)
                items.Remove(slot);
        }

        //New Scene
        if (slot.goRef == null)
        {
            slot.LoadSpriteAndGameObject();
            slot.goRef.transform.SetParent(cacheInventoryItemTransform);
        }

        //Invoke the item effect
        slot.itemEffect.Invoke();

        return true;
    }

    public string ToJSON()
    {
        return JsonUtility.ToJson(new SerializableList<InventorySlot>(items));
    }

    public void SetItemsList(List<InventorySlot> rhs)
    {
        //Remove All Items first
        foreach (InventorySlot slot in items)
        {
            Destroy(slot.goRef);
        }
        //Overwrite data
        items = rhs;
        //Reload sprite and gameObject
        items.ForEach((x) => { x.LoadSpriteAndGameObject(); x.goRef.transform.SetParent(cacheInventoryItemTransform); });
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class CheckpointController : MonoBehaviour
{
    public InventoryManager inventoryManager;

    public void Save(Collider other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            string s = inventoryManager.ToJSON();
            if (FileManager.WriteToFile("playerdata.json", s))
            {
                Debug.Log("Save player data successful");
            }
        }
    }

    public void Load()
    {
        string s;
        if (FileManager.LoadFromFile("playerdata.json", out s))
        {
            SerializableList<InventorySlot> listRef = JsonUtility.FromJson<SerializableList<InventorySlot>>(s);
            inventoryManager.SetItemsList(listRef.list);
            Debug.Log("Load player data successful");
        }
    }

}

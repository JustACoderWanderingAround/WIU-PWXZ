using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEditor.Progress;

public class CheckpointController : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public static GameObject[] CheckPointsList;
    private Vector3 lastCheckpointPos;

    void Start()
    {
        // We search all the checkpoints in the current scene
        CheckPointsList = GameObject.FindGameObjectsWithTag("Checkpoint");
    }


    public void Save(Collider other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            string s = inventoryManager.ToJSON();
            //s += JsonUtility.ToJson(transform.position);

            if (FileManager.WriteToFile("playerdata.json", s))
            {
                Debug.Log("Save player data successful");
            }

            lastCheckpointPos = other.transform.position;
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

            if (lastCheckpointPos != null) 
            {
                transform.position = lastCheckpointPos;
            }
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.L))
    //    {
    //        Load();
    //    }
    //}

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEditor.Progress;
using UnityEngine.SceneManagement;

public class CheckpointController : MonoBehaviour
{
    public InventoryManager inventoryManager;
    private SceneManagement sceneManagement;
    public static GameObject[] CheckPointsList;
    private Vector3 lastCheckpointPos;

    void Start()
    {
        // We search all the checkpoints in the current scene
        CheckPointsList = GameObject.FindGameObjectsWithTag("Checkpoint");
        sceneManagement = SceneManagement.Instance;
    }


    public void Save(Collider other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            string s = inventoryManager.ToJSON();

            if (FileManager.WriteToFile("playerdata.json", s))
            {
                Debug.Log("Save player data successful");
            }

            PlayerPrefs.SetString("SceneName", SceneManager.GetActiveScene().name);
            PlayerPrefs.SetString("CheckpointPos", other.transform.position.ToString());
            PlayerPrefs.Save();
            Debug.Log(PlayerPrefs.GetString("SceneName"));
            Debug.Log(PlayerPrefs.GetString("CheckpointPos"));

            //string info = JsonUtility.ToJson(SceneManager.GetActiveScene().name);
            //info += JsonUtility.ToJson(other.transform.position);

            //if (FileManager.WriteToFile("checkpointinfo.json", info))
            //{
            //    Debug.Log("Save player data successful");
            //}


        }
    }

    public void Load()
    {
        DontDestroyOnLoad(Camera.main);
        sceneManagement.LoadScene(PlayerPrefs.GetString("SceneName"));
        transform.position = StringToVector3(PlayerPrefs.GetString("CheckpointPos"));
        transform.position = new Vector3 (transform.position.x + 2.0f, transform.position.y, transform.position.z);

        string s;
        if (FileManager.LoadFromFile("playerdata.json", out s))
        {
            SerializableList<InventorySlot> listRef = JsonUtility.FromJson<SerializableList<InventorySlot>>(s);
            inventoryManager.SetItemsList(listRef.list);
            Debug.Log("Load player data successful");
            //DontDestroyOnLoad(gameObject);


        }
       
    }

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

}

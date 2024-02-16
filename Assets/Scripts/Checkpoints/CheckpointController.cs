using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointController : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public GameObject inventoryCache;
    private SceneManagement sceneManagement;
    public List<ItemState> ItemsList { get; private set; }
    private Vector3 lastCheckpointPos;

    void Start()
    {
        // We search all the checkpoints in the current scene
       
        sceneManagement = SceneManagement.Instance;
        ItemsList = new List<ItemState>();
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

            foreach (Transform g in inventoryCache.transform)
            {
                ItemState itemState = new ItemState(g.gameObject.name, g.gameObject.GetInstanceID(), g.gameObject.activeSelf, g.transform.position, g.transform.rotation.x, g.transform.rotation.y, g.transform.rotation.z);
                ItemsList.Add(itemState);
            }


            foreach (GameObject item in GameObject.FindGameObjectsWithTag("Item"))
            {
                
                ItemState itemState = new ItemState(item.name,item.GetInstanceID(),item.activeSelf, item.transform.position, item.transform.rotation.x, item.transform.rotation.y, item.transform.rotation.z);
                ItemsList.Add(itemState);
            }
            
            string items = JsonUtility.ToJson(new SerializableList<ItemState> (ItemsList));

            if (FileManager.WriteToFile("scenedata.json", items))
            {
                Debug.Log("Save scene data successful");
            }

            PlayerPrefs.SetString("SceneName", SceneManager.GetActiveScene().name);
            PlayerPrefs.SetString("CheckpointPos", other.transform.position.ToString());
            PlayerPrefs.Save();
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

        string _s;
        if (FileManager.LoadFromFile("scenedata.json", out _s))
        {
            List<ItemState> listItems = JsonUtility.FromJson<SerializableList<ItemState>>(_s).list;
            Debug.Log("Load scene data successful");

            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Item");

            foreach (GameObject item in GameObject.FindGameObjectsWithTag("Item"))
            {
                foreach (ItemState itemState in listItems)
                {
                    if (item.name == itemState.name && !itemState.isActive)
                    {
                        Destroy(item);
                    }
                }
            }
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

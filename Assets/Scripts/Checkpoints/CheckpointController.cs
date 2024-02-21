using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;


public class CheckpointController : MonoBehaviour
{
    public static CheckpointController Instance;
    public InventoryManager inventoryManager;
    public GameObject inventoryCache;
    private SceneManagement sceneManagement;
    public List<ItemState> ItemsList { get; private set; }
    public List<EnemyState> EnemiesList { get; private set; }
    private Vector3 lastCheckpointPos;

    public PhotoAlbum photoAlbum;

    void Start()
    {
        // We search all the checkpoints in the current scene
       
        sceneManagement = SceneManagement.Instance;
        ItemsList = new List<ItemState>();
        EnemiesList = new List<EnemyState>();
        if (photoAlbum == null)
            photoAlbum = GetComponentInChildren<PhotoAlbum>();
        DontDestroyOnLoad(this);

        Instance = this;
        if (transform.parent == null)
            DontDestroyOnLoad(this);
    }



    public void Save(Collider other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            string s = inventoryManager.ToJSON();

            photoAlbum.SaveImage();

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

            GameObject[] enemiesList = GameObject.FindGameObjectsWithTag("Enemy");



            foreach (GameObject enemy in enemiesList)
            {
                if (enemy.GetComponent<AINavigation>() != null)
                {
                    EnemyState enemyState = new EnemyState(enemy.name, enemy.activeSelf, enemy.transform.position, enemy.transform.eulerAngles.x, enemy.transform.eulerAngles.y, enemy.transform.eulerAngles.z);
                    EnemiesList.Add(enemyState);
                }
            }

            string enemies = JsonUtility.ToJson(new SerializableList<EnemyState>(EnemiesList));

            if (FileManager.WriteToFile("enemiesdata.json", enemies))
            {
                Debug.Log("Save enemy data successful");
            }

            PlayerPrefs.SetString("SceneName", SceneManager.GetActiveScene().name);
            PlayerPrefs.SetString("CheckpointPos", other.transform.position.ToString());
            //PlayerPrefs.SetInt("Money", ShopItemController.Instance.GetMoney());
            PlayerPrefs.Save();
        }
    }

    public void Load()
    {
        StartCoroutine(LoadRoutine());
    }

    public IEnumerator LoadRoutine()
    {
        DontDestroyOnLoad(Camera.main);
        sceneManagement.LoadScene(PlayerPrefs.GetString("SceneName"));

        while (sceneManagement.isLoading)
        {
            yield return null;
        }

        photoAlbum.Reload();

        transform.position = StringToVector3(PlayerPrefs.GetString("CheckpointPos"));
        transform.position = new Vector3 (transform.position.x + 3.0f, transform.position.y, transform.position.z);

        string s;
        if (FileManager.LoadFromFile("playerdata.json", out s))
        {
            SerializableList<InventorySlot> listRef = JsonUtility.FromJson<SerializableList<InventorySlot>>(s);
            inventoryManager.SetItemsList(listRef.list);
            Debug.Log("Load player data successful");
        }

        string s1;
        if (FileManager.LoadFromFile("scenedata.json", out s1))
        {
            List<ItemState> listItems = JsonUtility.FromJson<SerializableList<ItemState>>(s1).list;
            Debug.Log("Load scene data successful");

           // GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Item");

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

        string s2;
        if (FileManager.LoadFromFile("enemiesdata.json", out s2))
        {
            List<EnemyState> listEnemies = JsonUtility.FromJson<SerializableList<EnemyState>>(s2).list;
            Debug.Log("Load enemies data successful");

            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (enemy.GetComponent<AINavigation>() != null)
                {
                    foreach (EnemyState enemyState in listEnemies)
                    {
                        enemy.transform.position = enemyState.position;
                        enemy.transform.Rotate(enemyState.rotationX, enemyState.rotationY, enemyState.rotationZ);
                        //Quaternion.Euler()
                    }
                }
            }
        }

        //Refresh Renderer List Each Load
        GetComponent<CameraCapture>().UpdateRendererList();

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

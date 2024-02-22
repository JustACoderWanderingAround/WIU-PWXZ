using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;


public class CheckpointController : MonoBehaviour
{
    public static CheckpointController Instance;
    public InventoryManager inventoryManager;
    public InventoryHandler inventoryHandler;
    public GameObject inventoryCache;
    private SceneManagement sceneManagement;
    public List<ItemState> ItemsList { get; private set; }
    public List<EnemyState> EnemiesList { get; private set; }

    public List<GameObject> EnemyObjectList { get; private set; }
    public PhotoAlbum photoAlbum;
    private Collider collided = null;
    private bool isSaveUIActive;
    [SerializeField]
    private GameObject saveUICanvas;

    void Start()
    {
        // We search all the checkpoints in the current scene

        sceneManagement = SceneManagement.Instance;
        ItemsList = new List<ItemState>();
        EnemiesList = new List<EnemyState>();

        DontDestroyOnLoad(this);

        Instance = this;
        if (transform.parent == null)
            DontDestroyOnLoad(this);
    }

    public void SetSaveUIActive(Collider other)
    {
        isSaveUIActive = true;
        saveUICanvas.SetActive(isSaveUIActive);
        if (saveUICanvas.activeSelf)
        {
            collided = other;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = saveUICanvas.activeSelf;
        }
    }

    public void SetSaveUIInactive()
    {
        isSaveUIActive = false;
        saveUICanvas.SetActive(isSaveUIActive);
        if (!saveUICanvas.activeSelf)
        {
            collided = null;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = saveUICanvas.activeSelf;
        }
    }


    public void Save()
    {

        string s = inventoryManager.ToJSON();

        if (FileManager.WriteToFile("playerdata.json", s))
        {
            Debug.Log("Save player data successful");
        }

        foreach (InventorySlot g in inventoryManager.items)
        {
            ItemState itemState = new ItemState(g.goRef.name, g.goRef.GetInstanceID(), g.goRef.activeSelf, g.goRef.transform.position, g.goRef.transform.rotation.x, g.goRef.transform.rotation.y, g.goRef.transform.rotation.z);
            ItemsList.Add(itemState);
        }

        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Item"))
        {
            if (ItemsList.Find((x) => x.id == item.GetInstanceID()) == null)
            {
                ItemState itemState = new ItemState(item.name, item.GetInstanceID(), item.activeSelf, item.transform.position, item.transform.rotation.x, item.transform.rotation.y, item.transform.rotation.z);
                ItemsList.Add(itemState);
            }
        }

        string items = JsonUtility.ToJson(new SerializableList<ItemState>(ItemsList));

        if (FileManager.WriteToFile("scenedata.json", items))
        {
            Debug.Log("Save scene data successful");
        }

        GameObject[] enemiesList = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemiesList)
        {
            AINavigation aiHandler = enemy.GetComponent<AINavigation>();
            if (aiHandler != null)
            {
                //Check which type of enemy it is
                string workerType = "NULL";
                int waypoint = 0;
                EnemyWorker wh = enemy.GetComponent<EnemyWorker>();
                Guard gh = enemy.GetComponent<Guard>();

                if (wh != null)
                {
                    workerType = "EnemyWorker";
                    waypoint = wh.WaypointIndex;
                }
                else if (gh)
                {
                    workerType = "Guard";
                    waypoint = gh.WaypointIndex;
                }

                EnemyState enemyState = new EnemyState(enemy.name, enemy.activeSelf, enemy.transform.localPosition, enemy.transform.eulerAngles, workerType, waypoint);
                EnemiesList.Add(enemyState);
            }
        }

        string enemies = JsonUtility.ToJson(new SerializableList<EnemyState>(EnemiesList));

        if (FileManager.WriteToFile("enemiesdata.json", enemies))
        {
            Debug.Log("Save scene data successful");
        }

        PlayerPrefs.SetString("SceneName", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetString("CheckpointPos", transform.position.ToString());
        //PlayerPrefs.SetInt("Money", ShopItemController.Instance.GetMoney());
        PlayerPrefs.Save();

    }
    public void Load()
    {
        StartCoroutine(LoadRoutine());
    }

    public IEnumerator LoadRoutine()
    {
        DontDestroyOnLoad(Camera.main);
        if (PlayerPrefs.GetString("SceneName") == null)
        {
            yield break;
        }

        while (sceneManagement.isLoading) { yield return null; }

        sceneManagement.LoadScene(PlayerPrefs.GetString("SceneName"));

        while (sceneManagement.isLoading)
        {
            yield return null;
        }

        Time.timeScale = 0f;
        transform.position = StringToVector3(PlayerPrefs.GetString("CheckpointPos"));
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        string s;
        if (FileManager.LoadFromFile("playerdata.json", out s))
        {
            SerializableList<InventorySlot> listRef = JsonUtility.FromJson<SerializableList<InventorySlot>>(s);
            inventoryManager.SetItemsList(listRef.list);
            inventoryHandler.UpdateInventoryItemSlot();
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



        int i = 0;
        string s2;
        if (FileManager.LoadFromFile("enemiesdata.json", out s2))
        {
            List<EnemyState> listEnemies = JsonUtility.FromJson<SerializableList<EnemyState>>(s2).list;
            Debug.Log("Load enemies data successful");
            EnemyObjectList = new List<GameObject>();

            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (enemy.GetComponent<AINavigation>() != null)
                {
                    EnemyObjectList.Add(enemy);
                }
            }



            foreach (GameObject enemy in EnemyObjectList)
            {
                enemy.transform.localPosition = listEnemies[i].position;
                enemy.transform.eulerAngles = listEnemies[i].eulerAngles;
                switch (listEnemies[i].Type)
                {
                    case "EnemyWorker":
                        enemy.GetComponent<EnemyWorker>().WaypointIndex = listEnemies[i].waypointIndex;
                        break;
                    case "Guard":
                        enemy.GetComponent<Guard>().WaypointIndex = listEnemies[i].waypointIndex;
                        break;
                }
                i++;
            }
        }
        Debug.Log("LoadDone");
        Time.timeScale = 1f;
        SetSaveUIInactive();
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

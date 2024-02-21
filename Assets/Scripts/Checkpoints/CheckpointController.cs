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

    public List<GameObject> EnemyObjectList { get; private set; }
    private Collider collided = null;
    private bool isSaveUIActive;
    [SerializeField]
    private GameObject saveUICanvas;

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

    public void SetSaveUIActive(Collider other)
    {
        isSaveUIActive = !isSaveUIActive;
        saveUICanvas.SetActive(isSaveUIActive);
        if (saveUICanvas.activeSelf) {
            collided = other;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = saveUICanvas.activeSelf;
        }
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
                Debug.Log("Save enemy data successful");
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
        sceneManagement.LoadScene(PlayerPrefs.GetString("SceneName"));

        while (sceneManagement.isLoading)
        {
            yield return null;
        }

        photoAlbum.Reload();

        transform.position = StringToVector3(PlayerPrefs.GetString("CheckpointPos"));
        transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);

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



        int i = 0;
        string s2;
        if (FileManager.LoadFromFile("enemiesdata.json", out s2))
        {
            List<EnemyState> listEnemies = JsonUtility.FromJson<SerializableList<EnemyState>>(s2).list;
            Debug.Log("Load enemies data successful");
            EnemyObjectList = new List<GameObject>();

            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                GameObject child = enemy.transform.GetChild(0).gameObject;

                foreach (EnemyState enemyState in listEnemies)
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
                //foreach (EnemyState enemyState in listEnemies)
                //    {
                //        enemy.transform.position = enemyState.position;
                //        enemy.transform.Rotate(enemyState.rotationX, enemyState.rotationY, enemyState.rotationZ);
                //        //Quaternion.Euler()
                //        i++;
                //    }
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

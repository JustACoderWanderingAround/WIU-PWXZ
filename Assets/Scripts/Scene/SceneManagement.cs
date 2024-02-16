using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

//Created By: Tan Xiang Feng Wayne
public class SceneManagement : MonoBehaviour
{
    private static SceneManagement instance = null;
    public static SceneManagement Instance { get {
            if (instance == null)
            {
                GameObject newGO = new GameObject("SceneManager");
                instance = newGO.AddComponent<SceneManagement>();
                DontDestroyOnLoad(newGO);
            }
            return instance;
        } }
    [Header("Scene Data")]
    public List<string> sceneNames;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }   
        DontDestroyOnLoad(this);
        instance = this;
    }

    public void LoadScene(string name)
    {
        StartCoroutine(LoadSceneRoutine(name));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        AsyncOperation asyncHandler = SceneManager.LoadSceneAsync(sceneName);

        //When the async progress is not done
        while (!asyncHandler.isDone)
        {
            Debug.Log(string.Format("Load Scene {0} Progress: {1:P2}", sceneName, asyncHandler.progress));
            yield return null;
        }

        //Default set game time scale to 1 when a new scene is loaded
        Time.timeScale = 1;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
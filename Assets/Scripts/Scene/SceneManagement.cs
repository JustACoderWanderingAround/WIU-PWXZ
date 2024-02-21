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

    private AsyncOperation asyncHandler = null;

    private Coroutine loadSceneRoutine = null;
    
    public bool isLoading { get => asyncHandler != null; }

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
        if (loadSceneRoutine != null)
        {
            Debug.LogWarning("Loading Scene. Please wait before Loading another Scene");
            return;
        }
        loadSceneRoutine = StartCoroutine(LoadSceneRoutine(name));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        asyncHandler = SceneManager.LoadSceneAsync(sceneName);

        if (asyncHandler == null)
            yield break;

        //When the async progress is not done
        while (!asyncHandler.isDone)
        {
            Debug.Log(string.Format("Load Scene {0} Progress: {1:P2}", sceneName, asyncHandler.progress));
            yield return null;
        }
        asyncHandler = null;

        //Default set game time scale to 1 when a new scene is loaded
        Time.timeScale = 1;
        loadSceneRoutine = null;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
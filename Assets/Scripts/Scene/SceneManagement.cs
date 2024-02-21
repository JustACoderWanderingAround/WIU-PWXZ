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
    
    public bool isLoading { get; private set; }

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
        isLoading = true;
        Time.timeScale = 0;
        //Doing Scene Transitions
        Volume globalVolume = FindObjectOfType<Volume>();

        if (globalVolume != null)
        {
            DissolvePostProcessing dPP = null;
            globalVolume.sharedProfile.TryGet<DissolvePostProcessing>(out dPP);

            if (dPP != null)
            {
                float timePassed = 0f;
                while (timePassed < 1f)
                {
                    timePassed += Time.unscaledDeltaTime;
                    dPP.Progress.SetValue(new FloatParameter(Mathf.Min(timePassed, 1f)));
                    yield return null;
                }
            }
            else
            {
                Debug.Log("No Dissolve Post Processing Attached");
            }
        }

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
        loadCoroutine = null;
        isLoading = false;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
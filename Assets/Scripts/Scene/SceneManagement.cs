using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

//Created By: Tan Xiang Feng Wayne
public class SceneManagement : MonoBehaviour
{
    private static SceneManagement instance = null;
    public static SceneManagement Instance { get {
            if (instance == null)
            {
                GameObject newGO = Instantiate(Resources.Load("Prefabs/SceneManager") as GameObject, null);
                newGO.name = "SceneManager";
                newGO.SetActive(true);
                DontDestroyOnLoad(newGO);
            }
            return instance;
        } }
    [Header("Scene Data")]
    public List<string> sceneNames;

    private AsyncOperation asyncHandler = null;
    private Coroutine loadCoroutine = null;

    [Header("Canvas")]
    public TMPro.TMP_Text loadingText;
    
    public bool isLoading { get; private set; }

    private System.Action onSceneLoaded;

    public void OnSceneLoaded(System.Action onSceneLoad)
    {
        if (isLoading)
            onSceneLoaded += onSceneLoad;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }   
        DontDestroyOnLoad(this);
        instance = this;
        loadingText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void LoadScene(string name)
    {
        if (loadCoroutine == null)
            loadCoroutine = StartCoroutine(LoadSceneRoutine(name));
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
        loadingText.text = "Loading... 0%";
        loadingText.gameObject.SetActive(true);

        if (asyncHandler == null)
            yield break;

        //When the async progress is not done
        while (!asyncHandler.isDone)
        {
            Debug.Log(string.Format("Load Scene {0}  Progress: {1:P2}", sceneName, asyncHandler.progress));
            loadingText.text = string.Format("Loading... {0:P2}", asyncHandler.progress);
            yield return null;
        }
        asyncHandler = null;
        onSceneLoaded?.Invoke();
        onSceneLoaded = null;
        loadingText.gameObject.SetActive(false);
        isLoading = false;

        //Doing Scene Transitions : ON GAME TIME
        globalVolume = FindObjectOfType<Volume>();

        if (globalVolume != null)
        {
            DissolvePostProcessing dPP = null;
            globalVolume.sharedProfile.TryGet<DissolvePostProcessing>(out dPP);

            if (dPP != null)
            {
                float timePassed = 0f;
                while (timePassed < 1f)
                {
                    timePassed += Time.deltaTime;
                    dPP.Progress.SetValue(new FloatParameter(1f - Mathf.Min(timePassed, 1f)));
                    yield return null;
                }
            }
            else
            {
                Debug.Log("No Dissolve Post Processing Attached");
            }
        }

        //Default set game time scale to 1 when a new scene is loaded
        Time.timeScale = 1;
        loadCoroutine = null;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void OnSceneLoadSetTimeScale(float timeScale)
    {
        onSceneLoaded += () => Time.timeScale = timeScale;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class LoadGameScript : MonoBehaviour
{
    [SerializeField]
    private Button newGameButton, loadGameButton;
    [SerializeField]
    private PhotoAlbum photoAlbum = null;
    // Start is called before the first frame update
    void Start()
    {
        string sceneName = PlayerPrefs.GetString("SceneName");
        if (string.IsNullOrWhiteSpace(sceneName))
            loadGameButton.interactable = false;

        newGameButton.onClick.AddListener(OnNewGame);
        loadGameButton.onClick.AddListener(OnLoadGame);
    }

    public void OnLoadGame()
    {
        string sceneName = PlayerPrefs.GetString("SceneName");
        SceneManagement.Instance.LoadScene("BasementLevel");
        if (sceneName == null)
        {
            SceneManagement.Instance.OnSceneLoaded(() => PlayerController.Instance.SetIsDisabled(0));
        }
        else
        {
            SceneManagement.Instance.OnSceneLoaded(() =>
            {
                CheckpointController handler = FindObjectOfType<CheckpointController>();
                handler.Load();
                handler.StartCoroutine(LoadRoutine());
                IEnumerator LoadRoutine()
                {
                    while (SceneManagement.Instance.isLoading || SceneManagement.Instance.GetActiveSceneName() != sceneName)
                    {
                        yield return null;
                    }
                    PlayerController.Instance.SetIsDisabled(0);
                }
            });
        }
    }

    public void OnNewGame()
    {
        //Clear all the existing datas
        FileManager.WriteToFile("playerdata.json", null);
        FileManager.WriteToFile("scenedata.json", null);
        FileManager.WriteToFile("enemiesdata.json", null);
        PlayerPrefs.DeleteKey("SceneName");
        PlayerPrefs.DeleteKey("CheckpointPos");
        PlayerPrefs.DeleteKey("Money");
        photoAlbum?.ClearImages();
        //Load New Scene
        SceneManagement.Instance.LoadScene("BasementLevel");
        SceneManagement.Instance.OnSceneLoaded(() => PlayerController.Instance.SetIsDisabled(0));
    }
}

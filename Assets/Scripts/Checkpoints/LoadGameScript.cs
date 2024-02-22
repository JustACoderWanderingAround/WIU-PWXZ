using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}

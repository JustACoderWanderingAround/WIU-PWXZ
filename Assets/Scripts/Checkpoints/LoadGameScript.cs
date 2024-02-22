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
        SceneManagement.Instance.LoadScene("BasementLevel");
        SceneManagement.Instance.OnSceneLoaded(() => FindObjectOfType<CheckpointController>().Load());
    }
}

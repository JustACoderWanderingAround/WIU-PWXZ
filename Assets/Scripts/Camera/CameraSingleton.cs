using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSingleton : MonoBehaviour
{
    public static CameraSingleton Instance = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);

        Instance = this;
    }
}

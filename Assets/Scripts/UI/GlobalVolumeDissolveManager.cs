using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;

public class GlobalVolumeDissolveManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!SceneManagement.Instance.isLoadingEffect)
        {
            Volume globalVolume = GetComponent<Volume>();
            globalVolume.weight = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

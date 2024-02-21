using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolumeController : MonoBehaviour
{
    private GameObject gameObject;
    private Volume vol;
    private ColorTintPostProcess colorTintPostProcess;
    bool isWaterEffectActive;

    // Start is called before the first frame update
    void Start()
    {
        gameObject = GameObject.FindWithTag("GlobalVolume");
        vol = gameObject.GetComponent<Volume>();
        vol.profile.TryGet(out colorTintPostProcess);
    }

    public void SetWaterEffect()
    {
        isWaterEffectActive = !isWaterEffectActive;
        colorTintPostProcess.active = isWaterEffectActive;


        //colorTintPostProcess.blendIntensity.value = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

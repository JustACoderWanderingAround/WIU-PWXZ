using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;

public class GlobalVolumeController : MonoBehaviour
{
    private GameObject gameObject;
    private Volume vol;
    private ColorTintPostProcess colorTintPostProcess;
    bool isWaterEffectActive;
    UnityEngine.Rendering.PostProcessing.Vignette m_Vignette;
    PostProcessVolume m_Volume;

    // Start is called before the first frame update
    void Start()
    {
        //m_Vignette = ScriptableObject.CreateInstance<UnityEngine.Rendering.PostProcessing.Vignette>();
        //m_Vignette.enabled.Override(true);
        //m_Vignette.intensity.Override(1f);
        // Use the QuickVolume method to create a volume with a priority of 100, and assign the vignette to this volume
        //m_Volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, m_Vignette);

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

    public void EnableWaterEffect()
    {
        isWaterEffectActive = true;
        colorTintPostProcess.active = isWaterEffectActive;
    }

    public void DisableWaterEffect()
    {
        isWaterEffectActive = false;
        colorTintPostProcess.active = isWaterEffectActive;
    }
}

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/ColorTint Post-Processing", typeof(UniversalRenderPipeline))]

public class ColorTintPostProcess : VolumeComponent, IPostProcessComponent
{
    public FloatParameter blendIntensity = new FloatParameter(1.0f);
    public ColorParameter overlayColor = new ColorParameter(Color.white);

    public bool IsActive() => true;
    public bool IsTileCompatible() => true;
}


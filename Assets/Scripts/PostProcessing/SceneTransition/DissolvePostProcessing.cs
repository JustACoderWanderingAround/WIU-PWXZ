using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//VolumeComponentMenuForRenderPipeline
//allows you to add commands to the Add Override popout menu on Volumes and
//specify for which render pipelines will be supported
[Serializable, VolumeComponentMenuForRenderPipeline("Custom/Dissolve Post-Processing", typeof(UniversalRenderPipeline))]
public class DissolvePostProcessing : VolumeComponent, IPostProcessComponent
{
    public FloatParameter Progress = new ClampedFloatParameter(0f, 0f, 1f);
    public BoolParameter isActive = new BoolParameter(false);

    public bool IsActive() => (bool)isActive && (float)Progress > 0f;
    public bool IsTileCompatible() => true;
}

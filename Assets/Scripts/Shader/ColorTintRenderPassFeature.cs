using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorTintRenderPassFeature : ScriptableRendererFeature
{
    private ColorPass bwPass;

    public override void Create()
    {
        bwPass = new ColorPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(bwPass);
    }

    class ColorPass : ScriptableRenderPass
    {
        Material _mat;
        int colorId = Shader.PropertyToID("_Temp");
        RenderTargetIdentifier src, color;

        public ColorPass() 
        { 
            if (!_mat)
            {
                _mat = CoreUtils.CreateEngineMaterial("Custom Post-Processing/ColorTint Post-Processing");
            }

            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) 
        {
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            src = renderingData.cameraData.renderer.cameraColorTarget;
            cmd.GetTemporaryRT(colorId, desc, FilterMode.Bilinear);
            color = new RenderTargetIdentifier(colorId);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData render)
        {
            CommandBuffer commandBuffer = CommandBufferPool.Get("ColorTintRenderPassFeature");
            VolumeStack volumes = VolumeManager.instance.stack;
            ColorTintPostProcess colorPP = volumes.GetComponent<ColorTintPostProcess>();

            if (colorPP.IsActive())
            {
                _mat.SetFloat("_Intensity", (float)colorPP.blendIntensity);
                _mat.SetColor("_OverlayColor", (Color)colorPP.overlayColor);
                Blit(commandBuffer, src, color, _mat, 0);
                Blit(commandBuffer, color, src);
            }

            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(colorId);
        }
    }
}

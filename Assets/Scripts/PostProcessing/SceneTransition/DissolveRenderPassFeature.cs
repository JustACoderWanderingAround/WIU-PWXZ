using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Custom ScriptableRendererFeature for a black and white post-processing effect
public class DissolveRenderPassFeature : ScriptableRendererFeature
{
    private DissolvePass dissolvePass;

    //Called to add render passes to the renderer
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //Enqueue the custom render pass
        renderer.EnqueuePass(dissolvePass);
    }

    //Called when the feature is created
    public override void Create()
    {
        dissolvePass = new DissolvePass();
    }

    //Custom render pass class
    class DissolvePass : ScriptableRenderPass
    {
        Material _mat; //Material for the black and white effect
        int dissolveId = Shader.PropertyToID("_Temp"); //Property ID for the temporary render target
        RenderTargetIdentifier src, dissolve; //Render target identifiers
        RenderTexture texSrc;
        //Constructor for the DissolvePass
        public DissolvePass()
        {
            if (!_mat)
            {
                _mat = CoreUtils.CreateEngineMaterial("Shader Graphs/Dissolve");
            }
            //Set the render pass event to execute before post processing
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        //Called when setting up the camera for rendering
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            //Get the camera target descriptor
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            //Set the src render target identifier
            src = renderingData.cameraData.renderer.cameraColorTarget;
            //Ceate a temporary render target and get its identifer
            cmd.GetTemporaryRT(dissolveId, desc, FilterMode.Bilinear);
            dissolve = new RenderTargetIdentifier(dissolveId);
        }

        //Execute the custom render pass
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer commandBuffer = CommandBufferPool.Get("DissolveRenderPassFeature");
            //Access the volume stack and get the BlackAndWHitePostProcess component
            VolumeStack volumes = VolumeManager.instance.stack;
            DissolvePostProcessing DissolvePP = volumes.GetComponent<DissolvePostProcessing>();

            //Get the camera target descriptor
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;

            if (texSrc != null)
                RenderTexture.ReleaseTemporary(texSrc);

            if (DissolvePP.IsActive())
            {
                texSrc = RenderTexture.GetTemporary(desc);
                commandBuffer.Blit(renderingData.cameraData.renderer.cameraColorTarget, texSrc);

                _mat.SetFloat("_Progress", (float)DissolvePP.Progress);
                _mat.SetTexture("_CameraView", texSrc);
                Blit(commandBuffer, src, dissolve, _mat, 0);
                Blit(commandBuffer, dissolve, src);
            }

            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        //Called when cleaning up the camera after rendering
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(dissolveId);
        }
    }
}

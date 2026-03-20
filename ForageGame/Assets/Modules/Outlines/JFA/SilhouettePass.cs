using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

public class SilhouettePass
{
    class PassData
    {
        public Material SilhouetteMaterial;
        public List<Renderer> renderersToOutline;
    }

    static readonly int s_ObjectIDPropID = Shader.PropertyToID("_ObjectID");
    
    public static TextureHandle BuildSilhouette(RenderGraph renderGraph, ContextContainer frameData, Material silhouetteMaterial, List<Renderer> renderersToOutline)
    {
        var resourceData = frameData.Get<UniversalResourceData>();
        var source = resourceData.activeColorTexture;
        var desc = renderGraph.GetTextureDesc(source);
        desc.colorFormat = GraphicsFormat.R8_UNorm;
        desc.depthBufferBits = 0;
        desc.msaaSamples = MSAASamples.None;
        desc.name = "_SilhouetteMask";

        if(renderersToOutline.Count == 0)
        {
            return TextureHandle.nullHandle;
        }

        TextureHandle outputTexture = renderGraph.CreateTexture(desc);


        using (var builder = renderGraph.AddRasterRenderPass<PassData>("SilhouettePass", out var passData))
        {
            passData.SilhouetteMaterial = silhouetteMaterial;
            passData.renderersToOutline = renderersToOutline;

            builder.SetRenderAttachment(outputTexture, 0, AccessFlags.Write);
            builder.AllowPassCulling(false);

            builder.SetRenderFunc((PassData passData, RasterGraphContext context) =>
            {
                Debug.Log("Executing SilhouettePass");
                var cmd = context.cmd;

                // Clear to 0 (background) 
                cmd.ClearRenderTarget(RTClearFlags.All, Color.black, 1f, 0);

                foreach (var renderer in passData.renderersToOutline)
                {
                    cmd.DrawRenderer(renderer, passData.SilhouetteMaterial, 0, 0);
                }
            });
        }

        return outputTexture;
    }

}
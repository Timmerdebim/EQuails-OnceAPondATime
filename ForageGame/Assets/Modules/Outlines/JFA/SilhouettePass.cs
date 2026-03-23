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

    private static Material _silhouetteMaterial;
    static Material GetMaterial()
    {
        if (_silhouetteMaterial != null) return _silhouetteMaterial;
        const string shaderName = "Hidden/SilhouetteMask";
        var shader = Shader.Find(shaderName);
        if (shader == null)
        {
            Debug.LogError($"Could not find shader {shaderName}");
            return null;
        }

        _silhouetteMaterial = CoreUtils.CreateEngineMaterial(shader);
        return _silhouetteMaterial;
    }

    public static TextureHandle BuildSilhouette(RenderGraph renderGraph, ContextContainer frameData, List<Renderer> renderersToOutline)
    {
        var resourceData = frameData.Get<UniversalResourceData>();
        var source = resourceData.activeColorTexture;
        var desc = renderGraph.GetTextureDesc(source);
        desc.colorFormat = GraphicsFormat.R16_UNorm;
        desc.depthBufferBits = 0;
        desc.msaaSamples = MSAASamples.MSAA4x;
        desc.name = "_SilhouetteMask";

        if(renderersToOutline.Count == 0)
        {
            return TextureHandle.nullHandle;
        }

        TextureHandle outputTexture = renderGraph.CreateTexture(desc);


        using (var builder = renderGraph.AddRasterRenderPass<PassData>("SilhouettePass", out var passData))
        {
            passData.renderersToOutline = renderersToOutline;
            passData.SilhouetteMaterial = GetMaterial();

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
        
        // Resolve MSAA
        TextureDesc resolvedDesc = desc;
        resolvedDesc.msaaSamples = MSAASamples.None;
        resolvedDesc.name = "SilhouetteMask_MSAAResolved";
        TextureHandle resolved = renderGraph.CreateTexture(resolvedDesc);
        BlitTexture.BlitFromTo(outputTexture, resolved, renderGraph, frameData, "Resolve MSAA");

        return resolved;
    }

}
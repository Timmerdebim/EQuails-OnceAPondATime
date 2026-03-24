using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
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

    public struct TextureSet
    {
        public TextureHandle colorTexture;
        public TextureHandle depthTexture;
    }

    public static TextureSet BuildSilhouette(RenderGraph renderGraph, ContextContainer frameData, List<Renderer> renderersToOutline)
    {
        var resourceData = frameData.Get<UniversalResourceData>();
        var source = resourceData.activeColorTexture;
        var desc = renderGraph.GetTextureDesc(source);
        desc.colorFormat = GraphicsFormat.R16_UNorm;
        desc.depthBufferBits = 0;
        desc.msaaSamples = MSAASamples.None;
        desc.name = "_SilhouetteMask";
        
        TextureDesc depthDesc = desc;
        depthDesc.name = "_SilhouetteMaskDepth";
        depthDesc.colorFormat = GraphicsFormat.R32_SFloat;
        
        TextureSet output = new TextureSet();
        output.colorTexture = TextureHandle.nullHandle;
        output.depthTexture = TextureHandle.nullHandle;

        if(renderersToOutline.Count == 0)
        {
            return output;
        }

        TextureHandle outputTexture = renderGraph.CreateTexture(desc);
        TextureHandle depthTexture = renderGraph.CreateTexture(depthDesc);
        
        using (var builder = renderGraph.AddRasterRenderPass<PassData>("SilhouettePass", out var passData))
        {
            passData.renderersToOutline = renderersToOutline;
            passData.SilhouetteMaterial = GetMaterial();

            builder.SetRenderAttachment(outputTexture, 0, AccessFlags.Write);
            builder.SetRenderAttachment(depthTexture, 1, AccessFlags.Write);
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

        output.colorTexture = resolved;
        output.depthTexture = depthTexture;
        return output;
    }

}
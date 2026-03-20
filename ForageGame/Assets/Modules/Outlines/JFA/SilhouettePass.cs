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
        public (Renderer, float)[] drawList;
    }

    static readonly int s_ObjectIDPropID = Shader.PropertyToID("_ObjectID");
    
    public static TextureHandle BuildSilhouette(RenderGraph renderGraph, ContextContainer frameData, Material silhouetteMaterial)
    {
        var resourceData = frameData.Get<UniversalResourceData>();
        var source = resourceData.activeColorTexture;
        var desc = renderGraph.GetTextureDesc(source);
        desc.colorFormat = GraphicsFormat.R8_UNorm;    // perfect for 0–1 ID mask
        desc.depthBufferBits = 0;
        desc.msaaSamples = MSAASamples.None;
        desc.name = "_SilhouetteMask";


        TextureHandle outputTexture = renderGraph.CreateTexture(desc);

        List<OutlineObject> objects = OutlineObject.All;
        if (objects.Count == 0) return TextureHandle.nullHandle;

        int totalRenderers = 0;
        foreach (var obj in objects)
            totalRenderers += obj.Renderers.Length;

        var drawList = new (Renderer, float)[totalRenderers];

        int rendererIdx = 0;
        for (int objIdx = 0; objIdx < objects.Count; objIdx++)
        {
            // IDs start at 1 (0 = background/empty)
            // Normalize into (0, 1] so it survives an R8 render texture
            float normalizedID = (objIdx + 1) / 255f;

            objects[objIdx].OutlineID = objIdx + 1;

            foreach (var r in objects[objIdx].Renderers)
            {
                drawList[rendererIdx] = (r, normalizedID);
                rendererIdx++;
            }
        }

        using (var builder = renderGraph.AddRasterRenderPass<PassData>("SilhouettePass", out var passData))
        {
            passData.SilhouetteMaterial = silhouetteMaterial;
            passData.drawList = drawList;

            builder.SetRenderAttachment(outputTexture, 0, AccessFlags.Write);
            builder.AllowPassCulling(false);

            builder.SetRenderFunc((PassData passData, RasterGraphContext context) =>
            {
                Debug.Log("Executing SilhouettePass");
                var cmd = context.cmd;

                // Clear to 0 (background) 
                cmd.ClearRenderTarget(RTClearFlags.All, Color.black, 1f, 0);

                // Use a MPB to ensure each renderer gets its own ObjectID without needing to create multiple material instances
                var mpb = new MaterialPropertyBlock();
                foreach (var (renderer, normalizedID) in passData.drawList)
                {
                    mpb.SetFloat(s_ObjectIDPropID, normalizedID);
                    renderer.SetPropertyBlock(mpb);
                    cmd.DrawRenderer(renderer, passData.SilhouetteMaterial, 0, 0);
                }
            });
        }

        return outputTexture;
    }

}
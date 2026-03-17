// DebugBlitPass.cs
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class DebugBlitPass : ScriptableRenderPass
{
    class PassData
    {
        public TextureHandle src;
    }

    TextureHandle textureToBlit;

    public DebugBlitPass(TextureHandle textureToBlit)
    {
        this.textureToBlit = textureToBlit;
        renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        var resourceData = frameData.Get<UniversalResourceData>();

        using (var builder = renderGraph.AddRasterRenderPass<PassData>("DebugBlitSilhouette", out var passData))
        {
            passData.src = textureToBlit;

            builder.UseTexture(passData.src, AccessFlags.Read);
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);
            builder.AllowPassCulling(false);

            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                Blitter.BlitTexture(ctx.cmd, data.src, new Vector4(1, 1, 0, 0), 0, false);
            });
        }
    }
}
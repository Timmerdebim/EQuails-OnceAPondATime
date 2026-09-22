using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using System.Collections.Generic;

public class ObstacleMaskFeature : ScriptableRendererFeature
{
    public Material maskMaterial;
    public RenderingLayerMask renderingLayerMask = 1; // set to your ObstacleMask layer in the Inspector

    class MaskPass : ScriptableRenderPass
    {
        Material material;
        uint mask;
        int passIndex;
        new string passName;

        static readonly List<ShaderTagId> tags = new List<ShaderTagId>
    {
        new ShaderTagId("UniversalForward"),
        new ShaderTagId("UniversalForwardOnly"),
        new ShaderTagId("SRPDefaultUnlit"),
    };

        public MaskPass(string name, Material mat, uint mask, int passIndex)
        {
            passName = name;
            material = mat;
            this.mask = mask;
            this.passIndex = passIndex;
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        class PassData
        {
            public RendererListHandle rendererListHandle;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
            {
                var renderingData = frameData.Get<UniversalRenderingData>();
                var cameraData = frameData.Get<UniversalCameraData>();
                var lightData = frameData.Get<UniversalLightData>();
                var resourceData = frameData.Get<UniversalResourceData>();

                var filterSettings = new FilteringSettings(RenderQueueRange.opaque, ~0)
                {
                    renderingLayerMask = mask
                };

                var drawSettings = RenderingUtils.CreateDrawingSettings(
                    tags, renderingData, cameraData, lightData, cameraData.defaultOpaqueSortFlags);
                drawSettings.overrideMaterial = material;
                drawSettings.overrideMaterialPassIndex = passIndex;

                var rendererListParams = new RendererListParams(renderingData.cullResults, drawSettings, filterSettings);
                passData.rendererListHandle = renderGraph.CreateRendererList(rendererListParams);

                builder.UseRendererList(passData.rendererListHandle);
                builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
                builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Read);
                builder.AllowPassCulling(false);

                builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
                {
                    context.cmd.DrawRendererList(data.rendererListHandle);
                });
            }
        }
    }

    MaskPass back, front, fill;

    public override void Create()
    {
        uint m = renderingLayerMask.value;
        back = new MaskPass("ObstacleMask_Back", maskMaterial, m, 0);
        front = new MaskPass("ObstacleMask_Front", maskMaterial, m, 1);
        fill = new MaskPass("ObstacleMask_Fill", maskMaterial, m, 2);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (maskMaterial == null) return;
        renderer.EnqueuePass(back);
        renderer.EnqueuePass(front);
        renderer.EnqueuePass(fill);
    }
}
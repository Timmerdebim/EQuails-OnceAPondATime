using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class JFAOutlineRenderPass : ScriptableRenderPass
{
    Material silhouetteMaterial;

    JFAOutlineRenderPass(Material silhouetteMaterial)
    {
        this.silhouetteMaterial = silhouetteMaterial;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        TextureHandle silhouetteTexture = SilhouettePass.BuildSilhouette(renderGraph, frameData, silhouetteMaterial);

    }
}

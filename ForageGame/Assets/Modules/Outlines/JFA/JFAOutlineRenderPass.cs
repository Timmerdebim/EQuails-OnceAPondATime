using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class JFAOutlineRenderPass : ScriptableRenderPass
{
    Material silhouetteMaterial;
    bool debugView = false;
    Material thresholdMaterial;
    public JFAOutlineRenderPass(Material silhouetteMaterial, Material thresholdMaterial, bool debugView)
    {
        this.silhouetteMaterial = silhouetteMaterial;
        this.thresholdMaterial = thresholdMaterial;
        this.debugView = debugView;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        CameraType cameraType = cameraData.camera.cameraType;
        if (cameraType != CameraType.Game && cameraType != CameraType.SceneView)
            return;

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        TextureHandle silhouetteTexture = SilhouettePass.BuildSilhouette(renderGraph, frameData, silhouetteMaterial);
        if (!silhouetteTexture.IsValid())
        {
            // If the silhouette texture isn't valid, it means there were no objects to render in the silhouette pass. In this case, we can skip the rest of the outline rendering process.
            return;
        }

        TextureHandle output = silhouetteTexture;
        if (debugView)
        {
            output = ThresholdTexture.Threshold(silhouetteTexture, 0.001f, renderGraph, thresholdMaterial);
            DebugBlitTexture.BlitTexture(output, renderGraph, frameData);
        }

    }
}

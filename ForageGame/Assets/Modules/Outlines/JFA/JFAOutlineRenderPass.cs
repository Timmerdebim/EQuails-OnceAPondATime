using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
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

        List<TextureHandle> silhouetteTextures = GetSilhouetteTextures(renderGraph, frameData);

        if (silhouetteTextures.Count == 0)
        {
            return; // No silhouettes to process, skip the rest of the pass
        }

        if (debugView)
        {
            TextureHandle output = ThresholdTexture.Threshold(silhouetteTextures[0], 0.001f, renderGraph, thresholdMaterial);
            DebugBlitTexture.BlitTexture(output, renderGraph, frameData);
        }

    }

    private List<TextureHandle> GetSilhouetteTextures(RenderGraph renderGraph, ContextContainer frameData)
    {
        List<TextureHandle> silhouetteTextures = new List<TextureHandle>();

        OutlineObject[] outlineObjects = OutlineObject.All.ToArray();

        foreach (OutlineObject outlineObject in outlineObjects)
        {
            List<Renderer> renderersToOutline = new List<Renderer>(outlineObject.Renderers);
            TextureHandle silhouetteTexture = SilhouettePass.BuildSilhouette(renderGraph, frameData, silhouetteMaterial, renderersToOutline);
            if (silhouetteTexture.IsValid())
            {
                silhouetteTextures.Add(silhouetteTexture);
            }
            else
            {
                Debug.LogWarning($"Silhouette texture for {outlineObject.gameObject.name} is not valid. This likely means there were no renderers to outline for this object.");
            }
        }

        return silhouetteTextures;
    }
}

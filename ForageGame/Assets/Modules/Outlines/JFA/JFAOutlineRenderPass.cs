using System.Collections.Generic;
using Modules.Outlines;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class JFAOutlineRenderPass : ScriptableRenderPass
{
    public JFAOutlineRenderPass()
    {
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        CameraType cameraType = cameraData.camera.cameraType;
        if (cameraType != CameraType.Game && cameraType != CameraType.SceneView)
            return;

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        OutlineObject[] outlineObjects = OutlineObject.All.ToArray();

        if (outlineObjects.Length == 0)
        {
            return;
        }

        foreach (OutlineObject outlineObject in outlineObjects)
        {
            SilhouettePass.TextureSet silhouetteTex = GetSilhouetteTexture(renderGraph, frameData, outlineObject);
            TextureHandle jfaTex = JFARenderPass.JFAPass(renderGraph, frameData, silhouetteTex.colorTexture);
            Outline_pass.OutlinePass(renderGraph, frameData, jfaTex, silhouetteTex.colorTexture, silhouetteTex.depthTexture, outlineObject.OutlineWidth, outlineObject.OutlineColor);
        }

    }
    
    private SilhouettePass.TextureSet GetSilhouetteTexture(RenderGraph renderGraph, ContextContainer frameData, OutlineObject outlineObject)
    {
        List<Renderer> renderersToOutline = new List<Renderer>(outlineObject.Renderers);
        SilhouettePass.TextureSet silhouetteTexture = SilhouettePass.BuildSilhouette(renderGraph, frameData, renderersToOutline);
        if (!silhouetteTexture.colorTexture.IsValid())
        {
            Debug.LogWarning($"Silhouette texture for {outlineObject.gameObject.name} is not valid. This likely means there were no renderers to outline for this object.");
        }
        return silhouetteTexture;
    }
}

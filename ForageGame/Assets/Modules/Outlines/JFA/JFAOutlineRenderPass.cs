using System.Collections.Generic;
using Modules.Outlines;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class JFAOutlineRenderPass : ScriptableRenderPass
{
    bool debugView = false;
    Color outlineColor = Color.white;
    private float outlineWidth = 1f;
    public JFAOutlineRenderPass(bool debugView,  Color outlineColor, float outlineWidth)
    {
        this.outlineColor = outlineColor;
        this.outlineWidth = outlineWidth;
        this.debugView = debugView;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        CameraType cameraType = cameraData.camera.cameraType;
        if (cameraType != CameraType.Game && cameraType != CameraType.SceneView)
            return;

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        List<SilhouettePass.TextureSet> silhouetteTextures = GetSilhouetteTextures(renderGraph, frameData);

        if (silhouetteTextures.Count == 0)
        {
            return; // No silhouettes to process, skip the rest of the pass
        }

        if (debugView)
        {
            TextureHandle output = ThresholdTexture.Threshold(silhouetteTextures[0].colorTexture, 0.001f, renderGraph);
            BlitTexture.BlitToScreen(output, renderGraph, frameData);
        }

        foreach (SilhouettePass.TextureSet silhouetteTexture in silhouetteTextures)
        {
            TextureHandle JFA_Tex = JFARenderPass.JFAPass(renderGraph, frameData, silhouetteTexture.colorTexture);
            Outline_pass.OutlinePass(renderGraph, frameData, JFA_Tex, silhouetteTexture.colorTexture, silhouetteTexture.depthTexture, outlineWidth, outlineColor);
            
            
            // TextureHandle composite = OutlineFinalComposite.Composite(renderGraph, frameData, resourceData.activeColorTexture, outlineTexture);
            
            // BlitTexture.BlitToScreen(composite, renderGraph, frameData);
        }

    }

    private List<SilhouettePass.TextureSet> GetSilhouetteTextures(RenderGraph renderGraph, ContextContainer frameData)
    {
        List<SilhouettePass.TextureSet> silhouetteTextures = new List<SilhouettePass.TextureSet>();

        OutlineObject[] outlineObjects = OutlineObject.All.ToArray();

        foreach (OutlineObject outlineObject in outlineObjects)
        {
            List<Renderer> renderersToOutline = new List<Renderer>(outlineObject.Renderers);
            SilhouettePass.TextureSet silhouetteTexture = SilhouettePass.BuildSilhouette(renderGraph, frameData, renderersToOutline);
            if (silhouetteTexture.colorTexture.IsValid())
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class JFA_Outline_Feature : ScriptableRendererFeature
{
    JFAOutlineRenderPass pass;

    public Material SilhouetteMaterial;
    public Material ThresholdMaterial;
    public bool debugView = false;
    public RenderPassEvent injectionPoint = RenderPassEvent.BeforeRenderingPostProcessing;

    private TextureHandle silhouetteTH;
    private List<TextureHandle> distanceFieldTHs;

    public override void Create()
    {
        //SilhouetteMaterial = CoreUtils.CreateEngineMaterial("Hidden/SilhouetteID");

        pass = new JFAOutlineRenderPass(SilhouetteMaterial, ThresholdMaterial, debugView);
        pass.renderPassEvent = injectionPoint;

    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }

    protected override void Dispose(bool disposing)
    {
        //CoreUtils.Destroy(SilhouetteMaterial);
    }


}

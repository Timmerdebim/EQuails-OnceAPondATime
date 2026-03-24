using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class JFA_Outline_Feature : ScriptableRendererFeature
{
    JFA_Outline_Main_Pass pass;
    
    public RenderPassEvent injectionPoint = RenderPassEvent.BeforeRenderingPostProcessing;

    private TextureHandle silhouetteTH;
    private List<TextureHandle> distanceFieldTHs;

    public override void Create()
    {
        pass = new JFA_Outline_Main_Pass();
        pass.renderPassEvent = injectionPoint;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}

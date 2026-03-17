// SilhouetteRendererFeature.cs
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[DisallowMultipleRendererFeature]
public class SilhouetteRendererFeature : ScriptableRendererFeature
{
    Material m_IDMaterial;
    SilhouettePass m_Pass;

    public bool debugView = false;
    DebugBlitPass m_DebugPass;

    public override void Create()
    {
        m_IDMaterial = CoreUtils.CreateEngineMaterial("Hidden/SilhouetteID");
        m_Pass = new SilhouettePass(m_IDMaterial, debugView);
        m_DebugPass = new DebugBlitPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_Pass);
        if (debugView)
            renderer.EnqueuePass(m_DebugPass);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_IDMaterial);
    }
}
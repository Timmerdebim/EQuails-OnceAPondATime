using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class JFA_feature : ScriptableRendererFeature
{

    JFA_Pass pass;
    Material JFA_Material = CoreUtils.CreateEngineMaterial("Hidden/JFA_step");

    public override void Create()
    {
        pass = new JFA_Pass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }

    protected override void Dispose(bool disposing)
    {
    }
}

using Modules.Outlines;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class Outline_pass
{
    private static Material _OutlineMaterial;
    static Material OutlineMaterial
    {
        get
        {
            const string shaderName = "Hidden/OutlineFromJFA";

            if (_OutlineMaterial != null) return _OutlineMaterial;

            var shader = Shader.Find(shaderName);
            if (shader == null)
            {
                Debug.LogError($"Could not find shader {shaderName}");
                return null;
            }

            _OutlineMaterial = CoreUtils.CreateEngineMaterial(shader);
            return _OutlineMaterial;
        }

    }
    
    public static TextureHandle OutlinePass(RenderGraph renderGraph, ContextContainer frameData,
        TextureHandle JFATexture, float outlineWidth, Color outlineColor)
    {
        TextureDesc desc = JFATexture.GetDescriptor(renderGraph);
        desc.name = "Outline Pass Output";
        TextureHandle output = renderGraph.CreateTexture(desc);
        
        MaterialPropertyBlock mpb = new MaterialPropertyBlock(); 
        mpb.SetFloat("_OutlineWidth", outlineWidth);
        mpb.SetColor("_OutlineColor", outlineColor);
        RenderFullscreenWithMaterial.RenderPass(renderGraph, JFATexture, output, OutlineMaterial, "Outline Pass", "_MainTex", mpb);
        return output; 
    }

}

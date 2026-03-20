using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Experimental.Rendering;

public class JFARenderPass
{
    private static Material _JFAInitMat;
    static Material JFAInitMat
    {
        get
        {
            const string shaderName = "Hidden/JFA_Init";

            if (_JFAInitMat != null) return _JFAInitMat;

            var shader = Shader.Find(shaderName);
            if (shader == null)
            {
                Debug.LogError($"Could not find shader {shaderName}");
                return null;
            }

            _JFAInitMat = CoreUtils.CreateEngineMaterial(shader);
            return _JFAInitMat;
        }

    }

    public static TextureHandle JFAPass(RenderGraph renderGraph, ContextContainer frameData, TextureHandle silhouetteTexture)
    {
        return UVPositionsPrepass(renderGraph, frameData, silhouetteTexture);
    }

    class PassData
    {
        public TextureHandle src;
        public TextureHandle target;
        public Material mat;
    }

    private static TextureHandle UVPositionsPrepass(RenderGraph renderGraph, ContextContainer frameData, TextureHandle silhouetteTexture)
    {
        TextureDesc desc = silhouetteTexture.GetDescriptor(renderGraph);
        desc.name = "JFA_Init_Texture";
        desc.colorFormat = GraphicsFormat.R8G8_SNorm;

        TextureHandle output = renderGraph.CreateTexture(desc);

        using (var builder = renderGraph.AddRasterRenderPass<PassData>("JFA_UV_Positions_Prepass", out var passData))
        {
            passData.src = silhouetteTexture;
            passData.mat = JFAInitMat;
            passData.target = output;

            builder.UseTexture(passData.src, AccessFlags.Read);
            builder.SetRenderAttachment(passData.target, 0, AccessFlags.Write);

            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                Debug.Log("Executing JFA_UV_Positions_Prepass");
                data.mat.SetTexture("_SilhouetteTex", data.src);
                ctx.cmd.DrawProcedural(Matrix4x4.identity, data.mat, 0,
                                       MeshTopology.Triangles, 3, 1);
            });
        }

        return output;
    }

    private TextureHandle JFA_Step(RenderGraph renderGraph, ContextContainer frameData, TextureHandle inputTexture, int stepSize)
    {
        float[][] offsets = new float[][]
        {
            new float[2] {0, 0 },
            new float[2] { -stepSize, -stepSize },
            new float[2] { 0, -stepSize },
            new float[2] { stepSize, -stepSize },
            new float[2] { -stepSize, 0 },
            new float[2] { stepSize, 0 },
            new float[2] { -stepSize, stepSize },
            new float[2] { 0, stepSize },
            new float[2] { stepSize, stepSize }
        };

        foreach (float[] offset in offsets)
        {
            // For each offset, sample the input texture and write to the output if it's a valid position (not background)
            // This will likely involve setting the offset as a shader parameter and doing a draw call similar to the UVPositionsPrepass
        }
    }
}
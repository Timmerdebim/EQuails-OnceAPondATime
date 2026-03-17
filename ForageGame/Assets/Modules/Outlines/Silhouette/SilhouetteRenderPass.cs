// SilhouettePass.cs
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class SilhouettePass_old : ScriptableRenderPass
{
    class PassData
    {
        public Material material;
        // Snapshot of objects to draw, taken during record time
        public (Renderer renderer, float normalizedID)[] drawList;
    }

    readonly bool m_DebugView;
    readonly Material m_IDMaterial;
    static readonly int s_SilhouetteTexID = Shader.PropertyToID("_SilhouetteTex");
    static readonly int s_ObjectIDPropID = Shader.PropertyToID("_ObjectID");

    public SilhouettePass_old(Material idMaterial, bool debugView)
    {
        m_DebugView = debugView;
        m_IDMaterial = idMaterial;
        renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        // Always create it so DebugBlitPass doesn't throw
        var contextItem = frameData.Create<SilhouetteContextItem>();

        var objects = OutlineObject.All;
        if (objects.Count == 0) return; // contextItem.silhouetteTex stays nullHandle

        var drawList = new (Renderer, float)[0];
        int totalRenderers = 0;
        foreach (var obj in objects)
            totalRenderers += obj.Renderers.Length;

        drawList = new (Renderer, float)[totalRenderers];
        int index = 0;

        for (int i = 0; i < objects.Count; i++)
        {
            // IDs start at 1 (0 = background/empty)
            // Normalize into (0, 1] so it survives an R8 render texture
            float normalizedID = (i + 1) / 255f;
            if (m_DebugView)
            {
                normalizedID = 1;
            }
            objects[i].OutlineID = i + 1;

            foreach (var r in objects[i].Renderers)
                drawList[index++] = (r, normalizedID);
        }

        var cameraData = frameData.Get<UniversalCameraData>();

        var colorDesc = cameraData.cameraTargetDescriptor;
        colorDesc.colorFormat = RenderTextureFormat.R8;
        colorDesc.depthBufferBits = 0;
        colorDesc.msaaSamples = 1;

        var depthDesc = cameraData.cameraTargetDescriptor;
        depthDesc.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.None;
        depthDesc.depthBufferBits = 16;
        depthDesc.msaaSamples = 1;

        TextureHandle colorHandle = UniversalRenderer.CreateRenderGraphTexture(
            renderGraph, colorDesc, "_SilhouetteTex", false, FilterMode.Point);

        contextItem.silhouetteTex = colorHandle;

        TextureHandle depthHandle = UniversalRenderer.CreateRenderGraphTexture(
            renderGraph, depthDesc, "_SilhouetteDepth", false);

        using (var builder = renderGraph.AddRasterRenderPass<PassData>("SilhouettePass", out var passData))
        {
            passData.material = m_IDMaterial;
            passData.drawList = drawList;

            builder.SetRenderAttachment(colorHandle, 0, AccessFlags.Write);
            builder.SetRenderAttachmentDepth(depthHandle, AccessFlags.Write);
            builder.AllowPassCulling(false);

            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                ctx.cmd.ClearRenderTarget(RTClearFlags.All, Color.black, 1f, 0);

                var mpb = new MaterialPropertyBlock();
                foreach (var (renderer, normalizedID) in data.drawList)
                {
                    mpb.SetFloat(s_ObjectIDPropID, normalizedID);
                    renderer.SetPropertyBlock(mpb);
                    ctx.cmd.DrawRenderer(renderer, data.material, 0, 0);
                }
            });
        }
    }
}
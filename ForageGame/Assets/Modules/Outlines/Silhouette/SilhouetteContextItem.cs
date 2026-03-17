// SilhouetteContextItem.cs
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

public class SilhouetteContextItem : ContextItem
{
    public TextureHandle silhouetteTex;
    public override void Reset() => silhouetteTex = TextureHandle.nullHandle;
}
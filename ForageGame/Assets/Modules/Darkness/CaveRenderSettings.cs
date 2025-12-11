using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class CaveRenderSettings : MonoBehaviour
{
    // Cache for restoring state
    private Material prevSkybox;
    private AmbientMode prevAmbientMode;
    private Color prevAmbientLight;
    private Color prevAmbientSkyColor;
    
    private void OnEnable()
    {
        // 1. Capture current settings
        prevSkybox = RenderSettings.skybox;
        prevAmbientMode = RenderSettings.ambientMode;
        prevAmbientLight = RenderSettings.ambientLight;
        prevAmbientSkyColor = RenderSettings.ambientSkyColor;

        // 2. Apply Cave settings
        RenderSettings.ambientMode = AmbientMode.Flat;
        // dark grey
        // RenderSettings.ambientLight = Color.gray * 0.5f;
        RenderSettings.ambientLight = Color.white;
        RenderSettings.ambientSkyColor = Color.black; 
        RenderSettings.skybox = null;

        // Force GI update to remove skybox influence immediately
        DynamicGI.UpdateEnvironment();
    }

    private void OnDisable()
    {
        // 3. Restore previous settings
        RenderSettings.skybox = prevSkybox;
        RenderSettings.ambientMode = prevAmbientMode;
        RenderSettings.ambientLight = prevAmbientLight;
        RenderSettings.ambientSkyColor = prevAmbientSkyColor;

        DynamicGI.UpdateEnvironment();
    }
}
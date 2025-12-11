using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CaveVisionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera mainCamera;

    [Header("Settings")]
    [SerializeField] private float defaultRadius = 0.15f;
    [SerializeField] private float lanternRadius = 0.4f;
    [SerializeField] private float transitionSpeed = 2.0f;

    private Material material;
    private float currentRadius;
    private bool hasLantern = false;

    private static readonly int CenterID = Shader.PropertyToID("_Center");
    private static readonly int RadiusID = Shader.PropertyToID("_Radius");
    private static readonly int AspectID = Shader.PropertyToID("_AspectRatio");

    void Awake()
    {
        // Instantiate material to avoid modifying the asset
        material = GetComponent<Image>().material;
        currentRadius = defaultRadius;
    }

    void Update()
    {
        UpdateShaderParams();
        HandleRadiusInterpolation();
    }

    private void UpdateShaderParams()
    {
        if (playerTransform == null || mainCamera == null) return;

        // Map World Position to Viewport (0,0 to 1,1)
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(playerTransform.position);
        
        material.SetVector(CenterID, viewportPos);
        material.SetFloat(AspectID, (float)Screen.width / Screen.height);
    }

    private void HandleRadiusInterpolation()
    {
        float target = hasLantern ? lanternRadius : defaultRadius;
        currentRadius = Mathf.Lerp(currentRadius, target, Time.deltaTime * transitionSpeed);
        material.SetFloat(RadiusID, currentRadius);
    }

    // Public API for game logic
    public void SetLanternState(bool state) => hasLantern = state;
}
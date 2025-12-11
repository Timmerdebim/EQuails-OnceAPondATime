using UnityEngine;

[ExecuteAlways]
public class ViewConeController : MonoBehaviour
{
    public Transform target;
    public float radius = 0.5f;

    [Header("Raycast Settings")]
    public LayerMask obstacleMask = ~0;
    [Tooltip("Number of rays to cast per frame.")]
    public int rayCount = 100;

    [Header("Fade Settings")]
    public float fadeTime = 0.5f;

    [Header("Shader Targets")]
    // Values when the cone is fully VISIBLE (Blocked)
    public float visibleDitherScale = 1.0f;
    public float visibleEdgePower = 1.0f;

    // Values when the cone is HIDDEN (Clear)
    public float hiddenDitherScale = 0.0f;
    public float hiddenEdgePower = 0.0f;

    private MeshRenderer _coneMesh;
    private MaterialPropertyBlock _propBlock;
    
    private static readonly int PlayerPosID = Shader.PropertyToID("_GlobalPlayerPos");
    private static readonly int DitherScaleID = Shader.PropertyToID("_DitherScale");
    private static readonly int EdgePowerID = Shader.PropertyToID("_EdgeGradientPower");

    // Fading State
    private float _currentFade = 0f; // 0 = Hidden, 1 = Visible
    private float _targetFade = 0f;
    private float _fadeVelocity = 0f;

    void Awake()
    {
        _coneMesh = GetComponent<MeshRenderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    void LateUpdate()
    {
        if (target == null || Camera.main == null) return;

        HandleTransform();
        
        // 1. Run Logic
        CheckVisibility(rayCount);

        // 2. Smooth the fade value
        _currentFade = Mathf.SmoothDamp(_currentFade, _targetFade, ref _fadeVelocity, fadeTime);

        // 3. Optimize: Disable renderer if fully hidden to save fill rate
        if (_currentFade < 0.01f && _targetFade <= 0.01f)
        {
            if (_coneMesh.enabled) _coneMesh.enabled = false;
        }
        else
        {
            if (!_coneMesh.enabled) _coneMesh.enabled = true;
            ApplyShaderProperties();
        }
    }

    private void HandleTransform()
    {
        Vector3 camPos = Camera.main.transform.position;
        Vector3 playerPos = target.position;

        transform.position = (camPos + 2 * playerPos) / 3f;
        transform.LookAt(playerPos);
        transform.Rotate(90, 0, 0);
        
        float dist = Vector3.Distance(camPos, playerPos);
        transform.localScale = new Vector3(radius, dist / 3f, radius);

        Shader.SetGlobalVector(PlayerPosID, playerPos);
    }

    private void ApplyShaderProperties()
    {
        _coneMesh.GetPropertyBlock(_propBlock);

        // Lerp values based on _currentFade (0 to 1)
        float dither = Mathf.Lerp(hiddenDitherScale, visibleDitherScale, _currentFade);
        float edge = Mathf.Lerp(hiddenEdgePower, visibleEdgePower, _currentFade);

        _propBlock.SetFloat(DitherScaleID, dither);
        _propBlock.SetFloat(EdgePowerID, edge);

        _coneMesh.SetPropertyBlock(_propBlock);
    }

    public void CheckVisibility(int n)
    {
        if (n <= 0) return;

        Vector3 camPos = Camera.main.transform.position;
        Vector3 targetPos = target.position;
        Vector3 directionToCam = (camPos - targetPos).normalized;
        Quaternion lookRot = Quaternion.LookRotation(directionToCam);
        
        int reachedCount = 0;

        for (int i = 0; i < n; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            Vector3 offset = lookRot * new Vector3(randomCircle.x, randomCircle.y, 0);
            Vector3 origin = targetPos + offset;

            if (!Physics.Linecast(origin, camPos, obstacleMask))
            {
                reachedCount++;
            }
        }

        // Calculate ratio (0.0 to 1.0)
        float reachRatio = (float)reachedCount / n;

        // If ratio is 1.0 (All Clear), target is 0 (Hidden).
        // If ratio is 0.0 (All Blocked), target is 1 (Visible).
        _targetFade = 1.0f - reachRatio;
    }
}
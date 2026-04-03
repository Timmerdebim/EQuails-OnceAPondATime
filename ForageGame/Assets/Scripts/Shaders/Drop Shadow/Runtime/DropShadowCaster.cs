using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Attach to a sprite GameObject that lives in a 3D URP scene.
/// </summary>
public class DropShadowCaster : DecalProjector
{
    [SerializeField] private Material _material;
    [SerializeField]
    public Material Material
    {
        get => _material;
        set
        {
            _material = value;
            UpdateDecalProjector();
        }
    }
    [SerializeField] private float _length = 5.0f;
    public float Length
    {
        get => _length;
        set
        {
            _length = value;
            UpdateDecalProjector();
        }
    }
    [SerializeField] private float _initialRadius = 1.0f;
    public float InitialRadius
    {
        get => _initialRadius;
        set
        {
            _initialRadius = value;
            UpdateDecalProjector();
        }
    }
    [SerializeField] private float _finalRadius = 5.0f;
    public float FinalRadius
    {
        get => _finalRadius;
        set
        {
            _finalRadius = value;
            UpdateDecalProjector();
        }
    }
    [SerializeField] private float _softness = 0.5f;
    public float Softness
    {
        get => _softness;
        set
        {
            _softness = value;
            UpdateDecalProjector();
        }
    }
    [SerializeField] private Color _initialColor = new(0, 0, 0, 1);
    public Color InitialColor
    {
        get => _initialColor;
        set
        {
            _initialColor = value;
            UpdateDecalProjector();
        }
    }
    [SerializeField] private Color _finalColor = new(0, 0, 0, 0);
    public Color FinalColor
    {
        get => _finalColor;
        set
        {
            _finalColor = value;
            UpdateDecalProjector();
        }
    }

    public void UpdateDecalProjector()
    {
        float maxRadius = Mathf.Max(_initialRadius, _finalRadius);
        float normalizedInitialRadius = _initialRadius / maxRadius;
        float normalizedFinalRadius = _finalRadius / maxRadius;

        if (_material)
        {
            Material instanceMaterial = new(_material);
            instanceMaterial.SetFloat("_Softness", _softness);
            instanceMaterial.SetFloat("_Normalized_Initial_Radius", normalizedInitialRadius);
            instanceMaterial.SetFloat("_Normalized_Final_Radius", normalizedFinalRadius);
            instanceMaterial.SetColor("_Initial_Color", _initialColor);
            instanceMaterial.SetColor("_Final_Color", _finalColor);
            this.material = instanceMaterial;
        }

        this.pivot = new(0, 0, _length / 2);
        this.scaleMode = DecalScaleMode.InheritFromHierarchy;
        this.size = new(2 * maxRadius, 2 * maxRadius, _length);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;
        Vector3 tar = pos + _length * transform.forward;

        Handles.DrawWireDisc(pos, transform.forward, _initialRadius);
        Handles.DrawWireDisc(tar, transform.forward, _finalRadius);

        Handles.DrawLine(pos + _initialRadius * transform.up, tar + _finalRadius * transform.up);
        Handles.DrawLine(pos - _initialRadius * transform.up, tar - _finalRadius * transform.up);
        Handles.DrawLine(pos + _initialRadius * transform.right, tar + _finalRadius * transform.right);
        Handles.DrawLine(pos - _initialRadius * transform.right, tar - _finalRadius * transform.right);
    }
}
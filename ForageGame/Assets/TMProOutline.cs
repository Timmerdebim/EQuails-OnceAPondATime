using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TMProOutline : MonoBehaviour
{
    TMP_Text _text;
    [SerializeField] float outlineWidth = 1.0f;
    [SerializeField] Color outlineColor = Color.white;
    private void Start()
    {
        SetOutline();
    }

    public void SetColor(Color color)
    {
        outlineColor = color;
        SetOutline();
    }

    [ContextMenu("SetOutline")]
    private void SetOutline()
    {
        if (_text == null) _text = GetComponent<TMP_Text>();

        // 1. Get a copy of the material (Creates a new instance for this object)
        // If you use 'fontSharedMaterial', you will change it for EVERY text using this font!
        Material mat = _text.fontMaterial;

        // 2. Enable the Outline keyword (Required for some shaders to switch modes)
        mat.EnableKeyword("OUTLINE_ON");

        // 3. Set the Shader properties
        // Width is usually between 0.0 and 1.0
        mat.SetFloat("_OutlineWidth", outlineWidth);
        mat.SetColor("_OutlineColor", outlineColor);

        // 4. IMPORTANT: Force TMP to update its mesh boundaries
        // Without this, the outline might get "clipped" or cut off at the edges
        _text.UpdateMeshPadding();
    }
}

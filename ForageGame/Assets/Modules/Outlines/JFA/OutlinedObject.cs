using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class OutlineObject : MonoBehaviour
{
    public static readonly List<OutlineObject> All = new();

    Renderer[] m_Renderers;
    public Renderer[] Renderers => m_Renderers;
    
    [SerializeField] public float OutlineWidth = 10f;
    [SerializeField] public Color OutlineColor = Color.white;

    void OnEnable()
    {
        m_Renderers = GetComponentsInChildren<Renderer>();
        All.Add(this);
    }

    void OnDisable() => All.Remove(this);
}
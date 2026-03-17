// OutlineObject.cs
using System.Collections.Generic;
using UnityEngine;

public class OutlineObject : MonoBehaviour
{
    public static readonly List<OutlineObject> All = new();

    // Assigned each frame by the silhouette pass
    public int OutlineID { get; internal set; }

    Renderer[] m_Renderers;
    public Renderer[] Renderers => m_Renderers;

    void Awake() => m_Renderers = GetComponentsInChildren<Renderer>();
    void OnEnable() => All.Add(this);
    void OnDisable() => All.Remove(this);
}
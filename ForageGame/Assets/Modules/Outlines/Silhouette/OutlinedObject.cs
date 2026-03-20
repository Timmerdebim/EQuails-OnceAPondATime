using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class OutlineObject : MonoBehaviour
{
    public static readonly List<OutlineObject> All = new();
    public int OutlineID { get; internal set; }

    Renderer[] m_Renderers;
    public Renderer[] Renderers => m_Renderers;

    void OnEnable()
    {
        m_Renderers = GetComponentsInChildren<Renderer>();
        All.Add(this);
    }

    void OnDisable() => All.Remove(this);
}
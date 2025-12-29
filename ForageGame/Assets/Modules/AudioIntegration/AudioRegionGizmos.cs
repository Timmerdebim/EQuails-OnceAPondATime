using UnityEngine;

/// <summary>
/// Just gizmo collision colors for testing
/// </summary>

[RequireComponent(typeof(BoxCollider))]
public class AudioRegionGizmos : MonoBehaviour
{
    public Color normalColor;
    public Color activeColor;

    private BoxCollider col;
    private bool isPlayerInside = false;

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInside = false;
    }

    private void OnDrawGizmos()
    {
        if (col == null) return;

        Gizmos.color = isPlayerInside ? activeColor : normalColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(col.center, col.size);
    }
}

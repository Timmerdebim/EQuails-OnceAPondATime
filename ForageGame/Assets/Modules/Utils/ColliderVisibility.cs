using UnityEngine;

public class ColliderVisibility : MonoBehaviour
{
    private void Start()
    {
        foreach (var r in GetComponentsInChildren<MeshRenderer>())
            r.enabled = false; //hide the colliders in play mode
    }

    //but draw them again when turning gizmos on
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.35f);
        foreach (var r in GetComponentsInChildren<MeshRenderer>())
        {
            Gizmos.DrawMesh(
                r.GetComponent<MeshFilter>().sharedMesh,
                r.transform.position,
                r.transform.rotation,
                r.transform.lossyScale
            );
        }
    }
    #endif
}

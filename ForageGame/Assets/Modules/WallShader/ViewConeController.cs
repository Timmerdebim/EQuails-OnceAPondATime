using UnityEngine;

[ExecuteAlways]
public class ViewConeController : MonoBehaviour
{
    public Transform target;
    public float radius = 0.5f;

    // Cache the ID for performance
    private static readonly int PlayerPosID = Shader.PropertyToID("_GlobalPlayerPos");

    void LateUpdate()
    {
        if (target == null || Camera.main == null) return;

        Vector3 camPos = Camera.main.transform.position;
        Vector3 playerPos = target.position;
        
        // update the cylinder transform
        transform.position = (camPos + 2 * playerPos) / 3f;
        transform.LookAt(playerPos);
        transform.Rotate(90, 0, 0);
        float dist = Vector3.Distance(camPos, playerPos);
        transform.localScale = new Vector3(radius, dist / 3f, radius);

        // send pos to shader
        Shader.SetGlobalVector(PlayerPosID, playerPos);
    }
}
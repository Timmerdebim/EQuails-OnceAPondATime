using UnityEngine;

[ExecuteAlways]
public class ViewConeController : MonoBehaviour
{
    public Transform target; // Your Player
    public float radius = 0.5f;
    
    void LateUpdate()
    {
        // if (target == null || Camera.main == null) return;

        Vector3 camPos = Camera.main.transform.position;
        Vector3 playerPos = target.position;
        
        // Position exactly halfway between camera and player
        transform.position = (camPos + 2 * playerPos) / 3f;
        
        // Point the cylinder at the player
        transform.LookAt(playerPos);
        // Cylinders default to upright (Y-axis), so we rotate 90 deg to face forward
        transform.Rotate(90, 0, 0);

        // Scale it: 
        // X/Z = thickness of the tunnel
        // Y = Length (distance / 2 because Cylinder height is 2 units by default)
        float dist = Vector3.Distance(camPos, playerPos);
        transform.localScale = new Vector3(radius, dist / 4f, radius);
    }
}
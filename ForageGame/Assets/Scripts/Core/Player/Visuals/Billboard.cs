using UnityEngine;

//Just rotate to always face the camera ~Lars
//This is unused now, it gives a weird effect
public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 directionToCamera = Camera.main.transform.position - transform.position;
        directionToCamera.x = 0; // Keep upright
        
        if (directionToCamera != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToCamera);
        }
    }
}

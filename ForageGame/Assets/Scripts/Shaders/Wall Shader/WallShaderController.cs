using UnityEngine;

[ExecuteInEditMode]
public class WallShaderController : MonoBehaviour
{
    // Matches the variable name in the Shader
    private static readonly int PosID = Shader.PropertyToID("_GlobalPlayerPos");

    void Update()
    {
        // Send the player's position to ALL shaders containing this variable
        Shader.SetGlobalVector(PosID, transform.position);
    }
}
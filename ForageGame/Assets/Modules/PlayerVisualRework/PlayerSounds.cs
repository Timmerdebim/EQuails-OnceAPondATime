using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference footstepEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnFootstep()
    {
        var instance = FMODUnity.RuntimeManager.CreateInstance(footstepEvent);
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        instance.setParameterByName("TerrainType", GetTerrainType()); //yes IK this sucks, I can't pass a label for a labeled parameter, FMOD is great :)
        instance.start();
        instance.release();
    }

    //TODO: raycast to get current terrain type, now just default to grass
    private float GetTerrainType()
    {
        return 1f; //grass
    }
}

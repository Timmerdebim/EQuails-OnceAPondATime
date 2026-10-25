using UnityEngine;
using FMODUnity;

/// <summary>
/// Duplicate of FMOD's own supplied one just to have a separate name
/// Made espectially because:
/// - Tim's menu code sucks
/// - Unity animation events suck
/// 
/// ~Lars
/// </summary>
public class FMODEventEmitter2 : MonoBehaviour
{
    [SerializeField] EventReference eventReference;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayOneShot()
    {
        RuntimeManager.PlayOneShot(eventReference);
    }
}

using UnityEngine;

using FMODUnity;
using FMOD.Studio;

[System.Serializable]
public struct Syllable 
{ 
    public int label; //correspond to FMOD parameter value, should be an int but FMOD sucks and only takes string arguments, very cool!
    public float clipLength; //length of this clip, in seconds
    public float weight; //normalized 0-1 chance of this syllable being picked
    }

[CreateAssetMenu(fileName = "NpcVoice", menuName = "Audio/NpcVoice")]
public class NpcVoice : ScriptableObject
{
    public EventReference voiceEvent;
    public Syllable[] syllableVariants;
    public AnimationCurve syllableCountCurve; //how many characters of dialogue correspond to how many syllables spoken
    [Range(0f, 0.3f)] public float jitter = 0.05f; //random difference in next syllable, will likely go unused
}
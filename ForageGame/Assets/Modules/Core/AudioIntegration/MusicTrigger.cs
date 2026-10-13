using UnityEngine;
using System.Collections;
using FMODUnity;

[CreateAssetMenu(menuName = "Audio/Music Theme")]
public class MusicTheme : ScriptableObject
{
    public EventReference eventRef;
    public bool instant = false; //for home base, so it doesn't stop when sleeping, might be unused actually, build it into sleeping itself
    public float cooldownSeconds = 120f;
}

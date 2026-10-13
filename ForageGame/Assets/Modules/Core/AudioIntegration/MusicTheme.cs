using UnityEngine;
using System.Collections;
using FMODUnity;

[CreateAssetMenu(menuName = "Audio/Music Theme")]
public class MusicTheme : ScriptableObject
{
    public EventReference eventRef;
    public bool instant = false; //for home base, so it starts instantly and bypasses scheduling
    public float cooldownSeconds = 120f;
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public abstract class MusicTrigger : MonoBehaviour
{
    //Apparently Unity declarations are private instead of protected by default lol ~Lars
    protected BoxCollider col;

    [SerializeField] public FMODUnity.EventReference theme;  // FMOD event path

    [HideInInspector] public float lastPlayedTime = -Mathf.Infinity; //for theme cooldown

    public abstract float GetScheduledTime();

}

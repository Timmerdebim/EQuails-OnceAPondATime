using AudioIntegration;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using TDK.InteractionSystem;
using UnityEngine;
using NPC;

/// <summary>
/// JUST exists so the NPC head bopping is guaranteed to be in sync with the music
/// </summary>
public class NpcMusicCutscene : MonoBehaviour
{
    [Header("Music theme settings")]
    [SerializeField] private MusicTheme theme;
    [SerializeField] private float tempo = 1;
    private EventInstance instance;

    [SerializeField] private Interactable npcInteractable; //disables this one

    [SerializeField] private Animator animator;
    bool active = false;


    public void StartMusicCutscene()
    {
        Debug.Log($"[NpcMusicCutscene]: Starting music cutscene!");

        npcInteractable.DisableInteraction();

        GameplayMusicManager.Instance.StopAndRelease(); //stop the regular scheduled stuff
        GameplayMusicManager.Instance.AddCoolDownTime(theme);

        instance = RuntimeManager.CreateInstance(theme.eventRef);
        //RuntimeManager.AttachInstanceToGameObject(instance, transform.gameObject);
        animator.SetTrigger("Custom 1");
        animator.speed = tempo;

        instance.start();
        active = true;
    }

    //can also be called by the npc upon time passing
    public void EndMusicCutscene()
    {
        Debug.Log($"[NpcMusicCutscene]: Returning control to Npc!");
        animator.ResetTrigger("Custom 1");
        animator.SetTrigger("Interact");
        animator.speed = 1f;
        GameplayMusicManager.Instance.RestartScheduling();
        npcInteractable.EnableInteraction();
        active = false;
    }

    void Update()
    {
        if (active && !IsPlaying()) 
        { 
            Debug.Log($"[NpcMusicCutscene]: Theme finished!");
            EndMusicCutscene();
        }
    }

    //FMOD is restarted and requires this tomfoolery
    bool IsPlaying() 
    {
        if (!instance.isValid()) return false;
        FMOD.Studio.PLAYBACK_STATE eventState;   
        instance.getPlaybackState(out eventState);
        return eventState != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }


    void OnDestroy()
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
    }
}

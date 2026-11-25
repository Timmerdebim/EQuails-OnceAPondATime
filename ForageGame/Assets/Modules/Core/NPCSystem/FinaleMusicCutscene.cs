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
/// 
/// this one is just awkward since lyria and mosswick be there too
/// </summary>
public class FinaleMusicCutscene : MonoBehaviour
{
    [Header("Music theme settings")]
    [SerializeField] private MusicTheme theme;
    [SerializeField] private float tempo = 1;
    private EventInstance instance;

    [Header("Bracken")]
    [SerializeField] private Interactable npcInteractable; //disables this one

    [SerializeField] private Animator animator;

    [Header("Mosswick")]
    [SerializeField] private Interactable mosswickInteractable; //disables this one

    [SerializeField] private Animator mosswickAnimator;

    [Header("Lyria")]
    [SerializeField] private Interactable lyriaInteractable; //disables this one

    [SerializeField] private Animator lyriaAnimator;
    bool active = false;


    public void StartMusicCutscene()
    {
        Debug.Log($"[FinaleMusicCutscene]: Starting music cutscene!");

        npcInteractable.DisableInteraction();
        mosswickInteractable.DisableInteraction();
        lyriaInteractable.DisableInteraction();

        GameplayMusicManager.Instance.StopAndRelease(); //stop the regular scheduled stuff
        GameplayMusicManager.Instance.AddCoolDownTime(theme);

        instance = RuntimeManager.CreateInstance(theme.eventRef);
        //RuntimeManager.AttachInstanceToGameObject(instance, transform.gameObject);
        animator.SetTrigger("Custom 1");
        animator.speed = tempo;
        mosswickAnimator.SetTrigger("Custom 1");
        mosswickAnimator.speed = tempo;
        lyriaAnimator.SetTrigger("Custom 1");
        lyriaAnimator.speed = tempo;

        instance.start();
        active = true;
    }
    

    //can also be called by the npc upon time passing
    public void EndMusicCutscene()
    {
        Debug.Log($"[FinaleMusicCutscene]: Returning control to Npc!");
        animator.ResetTrigger("Custom 1");
        animator.SetTrigger("Interact");
        animator.speed = 1f;

        mosswickAnimator.ResetTrigger("Custom 1");
        mosswickAnimator.SetTrigger("Interact");
        mosswickAnimator.speed = 1f;

        lyriaAnimator.ResetTrigger("Custom 1");
        lyriaAnimator.SetTrigger("Interact");
        lyriaAnimator.speed = 1f;

        GameplayMusicManager.Instance.RestartScheduling();
        npcInteractable.EnableInteraction();
        mosswickInteractable.EnableInteraction();
        lyriaInteractable.EnableInteraction();

        active = false;
    }

    void Update()
    {
        if (active && !IsPlaying()) 
        { 
            Debug.Log($"[FinaleMusicCutscene]: Theme finished!");
            EndMusicCutscene();
        }
    }

    //FMOD is restarted and requires this tomfoolery
    bool IsPlaying() 
    {
        if (!instance.isValid()) return false;
        FMOD.Studio.PLAYBACK_STATE eventState;   
        instance.getPlaybackState(out eventState);
        return !(eventState == FMOD.Studio.PLAYBACK_STATE.STOPPED || eventState == FMOD.Studio.PLAYBACK_STATE.STOPPING);
    }

    public void StopMusicCutscene()
    {
        Debug.Log($"[FinaleMusicCutscene]: Canceling cutscene early!");
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        EndMusicCutscene();
    }


    void OnDestroy()
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
    }
}

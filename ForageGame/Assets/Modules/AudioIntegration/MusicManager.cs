using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the random trigger-ing of general music themes, also with events in mind
/// </summary>
public class MusicManager : MonoBehaviour
{
    //yes this is a singleton, fight me ~Lars
    public static MusicManager Instance { get; private set; }

    //list of themes that can be played rn, depending on what trigger regions we are in
    public List<GeneralThemeRegion> activeZones;
    [SerializeField] private FMOD.Studio.EventInstance currentTheme; 
    [SerializeField] private GeneralThemeRegion currentZone;

    public enum MusicManagerState
    {
        Scheduling,
        Playing,
        Suspended
    }

    [SerializeField] private MusicManagerState state = MusicManagerState.Scheduling;

    private void Awake() 
    { 
        //May only be one instance ofc
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    public void AddTheme(GeneralThemeRegion zone)
    {
        activeZones.Add(zone);
        print("Theme added to active list: " + zone);
        if(state == MusicManagerState.Scheduling) zone.StartScheduling();
    }

    public void RemoveTheme(GeneralThemeRegion zone)
    {
        activeZones.Remove(zone);
        if(state == MusicManagerState.Playing && currentZone == zone)
        {
            print("Player exited zone for current theme, fading out and restarting scheduling...");
            StopCoroutine(WaitForThemeEnd());
            currentTheme.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            state = MusicManagerState.Scheduling;
            RestartScheduling();
        }
    }

    private void StopAllScheduling()
    {
        foreach (var zone in activeZones)
            zone.StopScheduling();
    }

    private void RestartScheduling()
    {
        foreach (var zone in activeZones)
            zone.StartScheduling();
    }

    // Is public, so we can override anything if need be, i.e. make instant playback for a given room or anything ~Lars
    public bool PlayTheme(GeneralThemeRegion zone)
    {
        if (state != MusicManagerState.Scheduling || !activeZones.Contains(zone))
        {
            print("Theme play attempt ignored for zone: " + zone);
            return false; //something went wrong, or race condition!
        }
        //we acquired the lock, stop all other scheduling
        StopAllScheduling();
        print("Play " + zone.theme);
        
        //SANITY CHECK IF NOT SUSPENDED OR PLAYING SOMETHING RIGHT NOW
        currentZone = zone;
        currentTheme = FMODUnity.RuntimeManager.CreateInstance(zone.theme);
        currentTheme.start();

        state = MusicManagerState.Playing;
        StartCoroutine(WaitForThemeEnd());
        return true;
    }

    //FMOD is restarted and requires this tomfoolery
    bool IsPlaying() 
    {
	    FMOD.Studio.PLAYBACK_STATE eventState;   
	    currentTheme.getPlaybackState(out eventState);
	    return eventState != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

    private IEnumerator WaitForThemeEnd()
    {
        while (IsPlaying())
        {
            yield return null;
        }
        print("Theme done! Restarting scheduling...");
        state = MusicManagerState.Scheduling;
        RestartScheduling();
    }
}

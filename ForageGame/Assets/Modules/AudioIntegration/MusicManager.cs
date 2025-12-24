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
    public Dictionary<GeneralThemeRegion, float> zoneSchedule = new Dictionary<GeneralThemeRegion, float>();
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

    public void Update()
    {
        switch(state)
        {
            case MusicManagerState.Scheduling:
            {
                float now = Time.time;
                foreach(var (zone, schedTime) in zoneSchedule)
                {
                    if (schedTime <= now)
                    {
                        PlayTheme(zone);
                        break;
                    }
                }
                break;
            }
            case MusicManagerState.Playing:
            {
                if(!IsPlaying())
                {
                    print("Theme done! Restarting scheduling...");
                    state = MusicManagerState.Scheduling;
                    RestartScheduling();
                }
                break;
            }
            default: break; //no logic to be done while suspended
        }
    }

    public void AddTheme(GeneralThemeRegion zone)
    {
        zoneSchedule.Add(zone, zone.GetScheduledTime());
        print("Theme added to active list: " + zone + ", schedules for time: " + zoneSchedule[zone]);
    }

    public void RemoveTheme(GeneralThemeRegion zone)
    {
        zoneSchedule.Remove(zone);
        if(state == MusicManagerState.Playing && currentZone == zone)
        {
            print("Player exited zone for current theme, fading out and restarting scheduling...");
            state = MusicManagerState.Scheduling;
            //StopCoroutine(WaitForThemeEnd());
            currentTheme.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            RestartScheduling();
        }
    }

    private void RestartScheduling()
    {
        //yeah uhh I'm not allowed to modify the collection I'm iterating over, so I gotta deep copy it
        var keys = new List<GeneralThemeRegion>(zoneSchedule.Keys);

        foreach (var zone in keys)
        {
            zoneSchedule[zone] = zone.GetScheduledTime();
        }
        state = MusicManagerState.Scheduling;
    }

    // Is public, so we can override anything if need be, i.e. make instant playback for a given room or anything ~Lars
    public bool PlayTheme(GeneralThemeRegion zone)
    {
        if (state != MusicManagerState.Scheduling || !zoneSchedule.ContainsKey(zone))
        {
            print("Theme play attempt ignored for zone: " + zone);
            return false; //something went wrong, or race condition!
        }
        //we acquired the lock, stop all other scheduling
        state = MusicManagerState.Playing;
        print("Play " + zone.theme);
        
        //SANITY CHECK IF NOT SUSPENDED OR PLAYING SOMETHING RIGHT NOW
        currentZone = zone;
        currentTheme = FMODUnity.RuntimeManager.CreateInstance(zone.theme);
        currentTheme.start();

        //StartCoroutine(WaitForThemeEnd());
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

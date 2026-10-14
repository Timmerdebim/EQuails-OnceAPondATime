using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Linq;

namespace AudioIntegration
{
    /// <summary>
    /// Handles the random triggering of general music themes
    /// Cutscenes and other special events should suspend this one and play independently, and restart this one once done.
    /// </summary>
    public class GameplayMusicManager : MonoBehaviour, IManagedAudioSource
    {
        public static GameplayMusicManager Instance { get; private set; }
        public enum MusicManagerState
        {
            Suspended, //no scheduling or playback, used for when transitioning out of gameplay
            Scheduling,
            Playing //regular scheduled play, stops theme if no region is found for it
        }

        //list of unlocked themes, with their cooldown time.
        //TODO: SAVESAVE
        private Dictionary<MusicTheme, float> unlockedThemes = new Dictionary<MusicTheme, float>();
        //the current list of themes that could play in this area based on the active regions
        [SerializeField] private List<MusicTheme> activeRegionThemes = new List<MusicTheme>();
        [SerializeField] private MusicTheme currentTheme;
        [SerializeField] private FMOD.Studio.EventInstance currentThemeInstance; 

        [SerializeField] private MusicManagerState state = MusicManagerState.Suspended; //manager starts suspended to give the save manager time to teleport the player to the correct location before playing anything
        private float nextScheduleTime; //next time the manager will attempt to play something
        [SerializeField] private Vector2 scheduleDelayRange = new Vector2(10f, 40f);
        [SerializeField] private Vector2 priorityScheduleDelayRange = new(4f, 8f);
        private MusicTheme priorityTheme; //these are themes that are tried first, mostly used for newly unlocked themes.

        void OnEnable()  => AudioManager.Instance.Register(this);
        void OnDisable() => AudioManager.Instance.Unregister(this);

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


        #region Scheduling

        private List<MusicTheme> GetEligibleThemes()
        {
            return activeRegionThemes.Where(t => unlockedThemes.ContainsKey(t) && !OnCooldown(t)).ToList();
        }
        
        bool OnCooldown(MusicTheme t) => unlockedThemes.TryGetValue(t, out var end) && Time.time < end;


        private void SetNewScheduleTime() => nextScheduleTime = Time.time + Random.Range(scheduleDelayRange.x, scheduleDelayRange.y);

        private void TrySchedule()
        {
            var pool = GetEligibleThemes();
            if (pool.Count == 0)
            { 
                SetNewScheduleTime();
                Debug.Log($"[GameplayMusicManager]: No eligible themes to play, retrying in {nextScheduleTime - Time.time} seconds");
                return; 
            }
            else if (priorityTheme != null && pool.Contains(priorityTheme)) //priority themes
            {
                PlayTheme(priorityTheme);
                priorityTheme = null;
            }
            else //regular scheduling
            {
                PlayTheme(pool[Random.Range(0, pool.Count)]);
            }
        }

        void Update()
        {
            switch (state)
            {
                case MusicManagerState.Scheduling:
                    if (Time.time >= nextScheduleTime) TrySchedule();
                    break;
                case MusicManagerState.Playing:
                    if (!IsPlaying()) 
                    { 
                        Debug.Log($"[GameplayMusicManager]: Theme finished! scheduling next theme...");
                        StopCurrentTheme();
                        SetNewScheduleTime();
                        state = MusicManagerState.Scheduling;
                    }
                    break;
                default: break; //no logic while suspended
            }
        }

        #endregion

        #region Playback and Control

        //Marks 'theme' as priority, meaning it will be played next if possible.
        public void QueuePriorityTheme(MusicTheme theme)
        {
            //sanity check
            if (state != MusicManagerState.Scheduling && state != MusicManagerState.Playing)
            {
                Debug.LogWarning($"[GameplayMusicManager]: Priority theme play attempt ignored for theme:  {theme.name}, musicmanager suspended!");
                return;
            }
            //stop any currently playing theme, prevent overlap
            StopCurrentTheme();
            Debug.Log($"[GameplayMusicManager]: Priority theme queued: {theme.name}");

            state = MusicManagerState.Scheduling;
            priorityTheme = theme;
            nextScheduleTime = Time.time + Random.Range(priorityScheduleDelayRange.x, priorityScheduleDelayRange.y); //manual schedule time that is shorter
        }

        //regular 
        private void PlayTheme(MusicTheme theme)
        {
            //sanity check
            if (state != MusicManagerState.Scheduling || !unlockedThemes.ContainsKey(theme))
            {
                Debug.LogWarning($"[GameplayMusicManager]: Theme play attempt ignored for theme:  {theme.name}");
                return;
            }
            StopCurrentTheme(); //should never have an effect, but just in case
            state = MusicManagerState.Playing;
            Debug.Log($"[GameplayMusicManager]: Scheduled theme started: {theme.name}");
        
            currentThemeInstance = FMODUnity.RuntimeManager.CreateInstance(theme.eventRef);
            currentThemeInstance.start();
            currentTheme = theme;

            //set cooldown timer
            unlockedThemes[theme] = Time.time + theme.cooldownSeconds;
        }

        //FMOD is restarted and requires this tomfoolery
        bool IsPlaying() 
        {
            if (!currentThemeInstance.isValid()) return false;
            FMOD.Studio.PLAYBACK_STATE eventState;   
            currentThemeInstance.getPlaybackState(out eventState);
            return eventState != FMOD.Studio.PLAYBACK_STATE.STOPPED;
        }

        private void StopCurrentTheme()
        {
            if(IsPlaying() && currentThemeInstance.isValid())
            {
                currentThemeInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                currentThemeInstance.release();
                currentThemeInstance = default;
                currentTheme = null;
            }
        }

        //called by SaveManager after game has finished loading
        public void RestartScheduling()
        {
            StopCurrentTheme(); //should never have an effect
            SetNewScheduleTime();
            state = MusicManagerState.Scheduling;
        }

        #endregion

        #region Region Management

        public void UnlockTheme(MusicTheme theme)
        {
            if (!unlockedThemes.ContainsKey(theme))
            {
                unlockedThemes.Add(theme, Time.time);
                Debug.Log($"[GameplayMusicManager] New Theme Unlocked: {theme.name}");
            }
        }

        public void RegionActivated(MusicTheme theme)
        {
            activeRegionThemes.Add(theme);
            if(theme.instant)
            {
                Debug.Log($"[GemeplayMusicManager]: newly activated theme {theme.name} is instant, playing now!");
                PlayTheme(theme);
            }
            else Debug.Log($"[GameplayMusicManager] Theme added to active list: {theme.name}");
        }

        public void RegionDeactivated(MusicTheme theme)
        {
            activeRegionThemes.Remove(theme);
            //If the playing theme is no longer in an active region
            if(state == MusicManagerState.Playing && !activeRegionThemes.Contains(currentTheme))
            {
                Debug.Log($"[GameplayMusicManager]: Player exited region for current theme {currentTheme}, stopping and restarting scheduling...");
                StopCurrentTheme();
                SetNewScheduleTime();
                state = MusicManagerState.Scheduling;
            }
        }

        #endregion





        /// <summary>
        /// Inherited from IManagedAudioSource
        /// </summary>
        public void StopAndRelease()
        {
            StopCurrentTheme();
            state = MusicManagerState.Suspended;
        }
    }
}

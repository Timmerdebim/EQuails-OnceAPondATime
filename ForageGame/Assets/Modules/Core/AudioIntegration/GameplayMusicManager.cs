using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Linq;

namespace AudioIntegration
{
    /// <summary>
    /// Handles the random triggering of general music themes, also with events in mind
    /// </summary>
    public class GameplayMusicManager : MonoBehaviour, IManagedAudioSource
    {
        public static GameplayMusicManager Instance { get; private set; }
        public enum MusicManagerState
        {
            Suspended, //no scheduling or playback, used for when transitioning out of gameplay
            Scheduling,
            Playing, //regular scheduled play, stops theme if no region is found for it
            SuspendedForEvent //does not have the region leave logic, only stops once event done or scene unloaded
        }

        //list of unlocked themes, with their cooldown time.
        //TODO: SAVESAVE
        private Dictionary<MusicTheme, float> unlockedThemes = new Dictionary<MusicTheme, float>();
        //the current list of themes that could play in this area based on the active regions
        private List<MusicTheme> activeRegionThemes = new List<MusicTheme>();
        [SerializeField] private MusicTheme currentTheme;
        [SerializeField] private FMOD.Studio.EventInstance currentThemeInstance; 

        [SerializeField] private MusicManagerState state = MusicManagerState.Scheduling;
        private float nextScheduleTime; //next time the manager will attempt to play something
        [SerializeField] private float minScheduleDelay = 10f;
        [SerializeField] private float maxScheduleDelay = 40f;

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


        private void SetNewScheduleTime() => nextScheduleTime = Time.time + Random.Range(minScheduleDelay, maxScheduleDelay);

        private void TrySchedule()
        {
            var pool = GetEligibleThemes();
            if (pool.Count == 0)
            { 
                SetNewScheduleTime();
                Debug.Log($"[GameplayMusicManager]: No eligible themes to play, retrying in {nextScheduleTime - Time.time} seconds");
                return; 
            }
            PlayTheme(pool[Random.Range(0, pool.Count)]);
        }

        void Update()
        {
            switch (state)
            {
                case MusicManagerState.Scheduling:
                    if (Time.time >= nextScheduleTime) TrySchedule();
                    break;
                case MusicManagerState.Playing: //yes, this is allowed apparently in C#
                case MusicManagerState.SuspendedForEvent: //leaving logic is done on Region leave and such
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

        //Event themes go until completion, no matter what (unless game close of course)
        //remember that UnityEvents may only have one argument, hence the roundabout call chain
        public void PlayEventTheme(MusicTheme theme)
        {
            //sanity check
            if ((state != MusicManagerState.Scheduling && state != MusicManagerState.Playing) || !unlockedThemes.ContainsKey(theme))
            {
                Debug.LogWarning($"[GameplayMusicManager]: Event theme play attempt ignored for theme:  {theme.name}, theme not unlocked or suspended!");
                return;
            }
            //stop any currently playing theme
            StopCurrentTheme();
            Debug.Log($"[GameplayMusicManager]: Event theme started: {theme.name}");
            state = MusicManagerState.SuspendedForEvent;
            currentThemeInstance = FMODUnity.RuntimeManager.CreateInstance(theme.eventRef);
            currentThemeInstance.start();
            currentTheme = theme;

            //set cooldown timer
            unlockedThemes[theme] = Time.time + theme.cooldownSeconds;
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

        #endregion

        #region Region Management

        public void UnlockNewTheme(MusicTheme theme)
        {
            if (!unlockedThemes.ContainsKey(theme))
            {
                unlockedThemes.Add(theme, Time.time);
                Debug.Log($"[GameplayMusicManager] New Theme Unlocked: {theme.name}");
            }
        }

        public void ThemeActivated(MusicTheme theme)
        {
            activeRegionThemes.Add(theme);
            if(theme.instant)
            {
                Debug.Log($"[GemeplayMusicManager]: newly activated theme {theme.name} is instant, playing as event theme!");
                PlayEventTheme(theme);
            }
            else Debug.Log($"[GameplayMusicManager] Theme added to active list: {theme.name}");
        }

        public void ThemeDeactivated(MusicTheme theme)
        {
            activeRegionThemes.Remove(theme);
            //If currently playing this theme
            if(state == MusicManagerState.Playing && currentTheme == theme)
            {
                Debug.Log($"[GameplayMusicManager]: Player exited region for current theme {theme.name}, stopping and restarting scheduling...");
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

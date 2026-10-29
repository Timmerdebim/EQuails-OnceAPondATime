using System.Threading.Tasks;
using TDK.PlayerSystem;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using AudioIntegration;

namespace Project.Menus
{
    public class PauseMenu : Menu
    {
        [Header("Connected Menus")]
        [SerializeField] private MenuManager _menuManager;
        [SerializeField] private Menu settingsMenu;

        [Header("Sfx")]
        [SerializeField] private EventReference openSfx;
        [SerializeField] private EventReference closeSfx;
        [SerializeField] private EventReference duckMusicAndAmbienceSnapshot;
        private EventInstance duckMusicAndAmbience;

        void Awake()
        {
            if (!duckMusicAndAmbienceSnapshot.IsNull) 
            {
                duckMusicAndAmbience = FMODUnity.RuntimeManager.CreateInstance(duckMusicAndAmbienceSnapshot);
            }
        }


        public override void OnEnteringMenu()
        {
            RuntimeManager.PlayOneShot(openSfx);
            if (duckMusicAndAmbience.isValid() && !IsPlaying())  duckMusicAndAmbience.start();  //to prevent 'double start' when returning from settings tab
        }

        bool IsPlaying() 
        {
            if (!duckMusicAndAmbience.isValid()) return false;
            FMOD.Studio.PLAYBACK_STATE eventState;   
            duckMusicAndAmbience.getPlaybackState(out eventState);
            return eventState != FMOD.Studio.PLAYBACK_STATE.STOPPED;
        }

        public override void OnExitingMenu()
        {
            RuntimeManager.PlayOneShot(closeSfx);
        }

        public override void Escape()
        {
            _ = GameplayController.Instance.ResumeGame();
            //NOTE: not on exiting menu, because that also triggers when entering settings!
            if (duckMusicAndAmbience.isValid())  duckMusicAndAmbience.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); //Return the audio level
        }

        

        // ------------ Buttons ------------

        public void OnSettingsClicked()
        {
            _ = _menuManager.ToMenu(settingsMenu);
        }

        public void OnRespawnClicked()
        {
            _ = Respawn();
        }

        private async Task Respawn()
        {
            await GameplayController.Instance.ResumeGame();
            if (duckMusicAndAmbience.isValid())  duckMusicAndAmbience.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); //Return the audio level
            await GameplayController.Instance.Death();
        }

        public void OnMainMenuClicked()
        {
            _ = MainMenuSequence();
        }

        private async Task MainMenuSequence()
        {
            await GameplayController.Instance.ResumeGame();
            if (duckMusicAndAmbience.isValid())  duckMusicAndAmbience.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); //Return the audio level
            await GameplayController.Instance.QuitToMainMenu();
        }

        public void OnQuitClicked()
        {
            GameplayController.Instance.QuitToDesktop();
        }

        // ------------ Functions ------------
    }
}
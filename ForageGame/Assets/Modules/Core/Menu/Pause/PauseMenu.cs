using System.Threading.Tasks;
using TDK.PlayerSystem;
using UnityEngine;
using FMODUnity;

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

        public override void OnEnteringMenu()
        {
            RuntimeManager.PlayOneShot(openSfx);
        }

        public override void OnExitingMenu()
        {
            RuntimeManager.PlayOneShot(closeSfx);
        }

        public override void Escape()
        {
            _ = GameplayController.Instance.ResumeGame();
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
            await GameplayController.Instance.Death();
        }

        public void OnMainMenuClicked()
        {
            _ = MainMenuSequence();
        }

        private async Task MainMenuSequence()
        {
            await GameplayController.Instance.ResumeGame();
            await GameplayController.Instance.QuitToMainMenu();
        }

        public void OnQuitClicked()
        {
            GameplayController.Instance.QuitToDesktop();
        }

        // ------------ Functions ------------
    }
}
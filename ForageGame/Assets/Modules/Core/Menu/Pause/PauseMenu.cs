using System.Threading.Tasks;
using TDK.PlayerSystem;
using UnityEngine;

namespace Project.Menus
{
    public class PauseMenu : Menu
    {
        [Header("Connected Menus")]
        [SerializeField] private MenuManager _menuManager;
        [SerializeField] private Menu settingsMenu;

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
            await _menuManager.ToMenu(null);
            await GameplayController.Instance.QuitToMainMenu();
        }

        public void OnQuitClicked()
        {
            GameplayController.Instance.QuitToDesktop();
        }

        // ------------ Functions ------------
    }
}
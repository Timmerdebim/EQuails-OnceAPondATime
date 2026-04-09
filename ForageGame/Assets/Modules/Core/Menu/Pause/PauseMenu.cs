using NUnit.Framework.Internal;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Menus
{
    public class PauseMenu : Menu
    {
        [Header("Connected Menus")]
        [SerializeField] private Menu settingsMenu;

        void Start()
        {
            MenuManager.Instance.ToMenu(this, false);
        }

        public override void Escape()
        {
            GameplayController.Instance.ResumeGame();
        }

        // ------------ Buttons ------------

        public void OnSettingsClicked()
        {
            MenuManager.Instance.ToMenu(settingsMenu, true);
        }

        public void OnMainMenuClicked()
        {
            GameplayController.Instance.QuitToMainMenu();
        }

        public void OnQuitClicked()
        {
            GameplayController.Instance.QuitToDesktop();
        }

        // ------------ Functions ------------
    }
}
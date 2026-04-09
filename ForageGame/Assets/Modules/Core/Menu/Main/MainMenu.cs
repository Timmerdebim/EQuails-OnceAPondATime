using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TDK.SaveSystem;

namespace Project.Menus
{
    public class MainMenu : Menu
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text continueText;

        [Header("Connected Menus")]
        [SerializeField] private Menu fileSelectMenu;
        [SerializeField] private Menu settingsMenu;
        [SerializeField] private Menu creditsMenu;

        void Start()
        {
            MenuManager.Instance.ToMenu(this, false);
        }

        public override void EnteringMenu()
        {
            base.EnteringMenu();
            RefreshVisuals();
        }

        public override void Escape()
        {
        }

        // ------------ Buttons ------------

        public void OnContinueClicked()
        {
            _ = AppController.Instance.ToWorld();
        }

        public void OnFileSelectClicked()
        {
            MenuManager.Instance.ToMenu(fileSelectMenu, true);
        }

        public void OnSettingsClicked()
        {
            MenuManager.Instance.ToMenu(settingsMenu, true);
        }

        public void OnCreditsClicked()
        {
            MenuManager.Instance.ToMenu(creditsMenu, true);
        }

        public void OnQuitClicked()
        {
            AppController.Instance.Quit();
        }

        // ------------ Functions ------------

        private void RefreshVisuals()
        {
            string worldId = PlayerPrefs.GetString("lastWorldUsed", null);

            if (worldId == null || !SaveServices.ExistsWorld(worldId))
                continueText.text = "New Game";
            else
                continueText.text = "Continue";
        }
    }
}
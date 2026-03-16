using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            GameManager.Instance.PlayGame();
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
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // ------------ Functions ------------

        private void RefreshVisuals()
        {
            int slotIndex = PlayerPrefs.GetInt("lastSlotIndexUsed", -1);

            if (slotIndex < 0 || !SaveSystem.SaveFileExists(slotIndex))
                continueText.text = "New Game";
            else
                continueText.text = "Continue";
        }
    }
}
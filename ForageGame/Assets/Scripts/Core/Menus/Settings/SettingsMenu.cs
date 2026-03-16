using UnityEngine;
using UnityEngine.UI;
using Project.Menus.Keybind;
using Project.Menus.Graphics;
using Project.Menus.Audio;

namespace Project.Menus
{
    public class SettingsMenu : Menu
    {
        [Header("Buttons")]
        [SerializeField] private Button graphicsButton;
        [SerializeField] private Button audioButton;
        [SerializeField] private Button keybindsButton;

        [Header("Connected Menus")]
        [SerializeField] private Menu mainMenu;
        [SerializeField] private Menu pauseMenu;
        [SerializeField] private Menu graphicsSettingsMenu;
        [SerializeField] private Menu audioSettingsMenu;
        [SerializeField] private Menu keybindSettingsMenu;

        private Menu currentSubMenu;

        public override void EnteringMenu()
        {
            base.EnteringMenu();

            graphicsButton.interactable = false;
            audioButton.interactable = true;
            keybindsButton.interactable = true;

            ToSubMenu(graphicsSettingsMenu);
        }

        public override void ExitingMenu()
        {
            base.ExitingMenu();

            GraphicsSettingsManager.Instance.SaveSettings();
            AudioSettingsManager.Instance.SaveSettings();
            KeybindSettingsManager.Instance.SaveSettings();
        }

        public override void Escape()
        {
            if (GameManager.Instance.state == GameState.PauseMenu)
                MenuManager.Instance.ToMenu(pauseMenu, true);
            else if (GameManager.Instance.state == GameState.MainMenu)
                MenuManager.Instance.ToMenu(mainMenu, true);
            else
                Debug.Log($"MENU: ERROR: Tried to escape the main menu when menu mode was none.");
        }

        // ------------ Buttons ------------

        public void OnGraphicsClicked()
        {
            graphicsButton.interactable = false;
            audioButton.interactable = true;
            keybindsButton.interactable = true;
            ToSubMenu(graphicsSettingsMenu);
        }

        public void OnAudioClicked()
        {
            graphicsButton.interactable = true;
            audioButton.interactable = false;
            keybindsButton.interactable = true;
            ToSubMenu(audioSettingsMenu);
        }

        public void OnKeybindsClicked()
        {
            graphicsButton.interactable = true;
            audioButton.interactable = true;
            keybindsButton.interactable = false;
            ToSubMenu(keybindSettingsMenu);
        }

        // ------------ Functions ------------

        public void ToSubMenu(Menu toMenu)
        {
            currentSubMenu?.ExitingMenu();
            currentSubMenu?.ExitedMenu();

            currentSubMenu = toMenu;

            toMenu?.EnteringMenu();
            toMenu?.EnteredMenu();
        }
    }
}
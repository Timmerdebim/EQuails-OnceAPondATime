using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Settings
{
    public int resolutionIndex = 0;
    public int qualityLevel = 2;
    public bool vsyncEnabled = true;
    public int targetFramerate = 60;
}

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

    public override void Escape()
    {
        if (mainMenu == null)
            MenuManager.Instance.ToMenu(pauseMenu, true);
        else
            MenuManager.Instance.ToMenu(mainMenu, true);
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
        if (currentSubMenu != null)
        {
            currentSubMenu.ExitingMenu();
            currentSubMenu.ExitedMenu();
            currentSubMenu.gameObject.SetActive(false);
        }

        currentSubMenu = toMenu;

        toMenu.gameObject.SetActive(true);
        toMenu.EnteringMenu();
        toMenu.EnteredMenu();
    }
}
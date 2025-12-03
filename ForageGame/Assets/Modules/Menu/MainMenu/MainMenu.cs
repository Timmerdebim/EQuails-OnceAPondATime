using UnityEngine;

public class MainMenu : MonoBehaviour, IMenu
{
    // ------------ MENU PANELS ------------
    [Header("Menu Panels")]
    [SerializeField] private FileSelectMenu fileSelectMenu;
    // [SerializeField] private IMenu settingsMenu; // other?
    [SerializeField] private AchievementsMenu achievementsMenu;
    [SerializeField] private CreditsMenu creditsMenu;

    // ------------ OTHER COMPONENTS ------------
    [Header("Misc")]
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject continueButton;

    // ------------ IMenu ------------

    void Awake()
    {
        OpenMenu();
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
        RefreshContinueButton();
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    // ------------ BUTTONS ------------

    public void NewGame_Button()
    {
        SaveSystem.NewSaveFile(1);
        SaveSystem.LoadSaveFile(1);
    }

    public void Continue_Button()
    {
        int lastSlot = PlayerPrefs.GetInt("LastSaveFileUsed", 1);
        SaveSystem.LoadSaveFile(lastSlot);
    }

    public void FileSelect_Button()
    {
        CloseMenu();
        fileSelectMenu.OpenMenu();
    }

    public void Settings_Button()
    {
        // TODO
    }

    public void Achievements_Button()
    {
        CloseMenu();
        achievementsMenu.OpenMenu();
    }

    public void Credits_Button()
    {
        CloseMenu();
        creditsMenu.OpenMenu();
    }

    public void Quit_Button()
    {
        Application.Quit();
    }

    // ------------ OTHER FUNCTIONS ------------

    private void RefreshContinueButton()
    {
        int lastSlot = PlayerPrefs.GetInt("LastUsedSlot", -1);
        bool newGame = lastSlot == -1;

        newGameButton.SetActive(newGame);
        continueButton.SetActive(!newGame);
    }
}

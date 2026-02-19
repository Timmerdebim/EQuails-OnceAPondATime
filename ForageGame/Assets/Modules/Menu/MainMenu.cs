using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [Header("Connected Menus")]
    [SerializeField] private Menu fileSelectMenu;
    [SerializeField] private Menu settingsMenu;
    [SerializeField] private Menu creditsMenu;
    [SerializeField] private Menu achievementsMenu;

    void Start()
    {
        MenuManager.Instance.ToMenu(this, false);
    }

    public override void Escape()
    {
    }

    // ------------ Buttons ------------

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

    public void OnAchievementsClicked()
    {
        MenuManager.Instance.ToMenu(achievementsMenu, true);
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
}
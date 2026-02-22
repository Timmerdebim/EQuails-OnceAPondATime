using NUnit.Framework.Internal;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        MenuManager.Instance.ResumeGame();
    }

    // ------------ Buttons ------------

    public void OnSettingsClicked()
    {
        MenuManager.Instance.ToMenu(settingsMenu, true);
    }

    public void OnMainMenuClicked()
    {
        MenuManager.Instance.ToMainMenu();
    }

    public void OnQuitClicked()
    {
        GameManager.Instance.QuitGame();
    }

    // ------------ Functions ------------
}
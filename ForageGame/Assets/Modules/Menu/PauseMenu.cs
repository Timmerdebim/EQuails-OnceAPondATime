using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenu : Menu
{
    [Header("Connected Menus")]
    [SerializeField] private Menu settingsMenu;

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
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // ------------ Functions ------------

    public void PauseGame()
    {
        MenuManager.Instance.PauseGame();
    }
}
using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    private Menu currentMenu;
    public MenuMode mode { get; private set; } = MenuMode.None;
    private Sequence seq;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ToMenu(Menu toMenu, bool doFade)
    {
        if (doFade) MenuTransition(currentMenu, toMenu);
        else DirectMenuTransition(currentMenu, toMenu);
    }

    private void DirectMenuTransition(Menu fromMenu, Menu toMenu)
    {
        if (fromMenu != null)
        {
            fromMenu.ExitingMenu();
            fromMenu.ExitedMenu();
            fromMenu.gameObject.SetActive(false);
        }

        currentMenu = toMenu;

        if (toMenu != null)
        {
            toMenu.gameObject.SetActive(true);
            toMenu.EnteringMenu();
            toMenu.EnteredMenu();
        }
    }

    private void MenuTransition(Menu fromMenu, Menu toMenu)
    {
        seq?.Kill();
        seq = DOTween.Sequence();

        if (fromMenu != null)
        {
            seq.AppendCallback(() => { fromMenu.ExitingMenu(); });
            seq.Append(fromMenu.canvasGroup.DOFade(0, fromMenu.fadeOutDuration));
            seq.AppendCallback(() =>
            {
                fromMenu.ExitedMenu();
                fromMenu.gameObject.SetActive(false);
            });
        }

        seq.AppendCallback(() => { currentMenu = toMenu; });

        if (toMenu != null)
        {
            seq.AppendCallback(() =>
            {
                toMenu.gameObject.SetActive(true);
                toMenu.EnteringMenu();
            });
            seq.Append(toMenu.canvasGroup.DOFade(1, toMenu.fadeInDuration));
            seq.AppendCallback(() => { toMenu.EnteredMenu(); });
        }
    }

    public void PauseGame()
    {
        GameManager.Instance.sceneLoader.LoadScenesByGroup(SceneGroup.Pause, () => { SetMenuMode(MenuMode.Pause); });
    }

    public void ResumeGame()
    {
        ToMenu(null, false);
        GameManager.Instance.sceneLoader.UnloadScenesByGroup(SceneGroup.Pause, () => { SetMenuMode(MenuMode.None); });
    }

    public void Escape()
    {
        if (currentMenu == null && mode == MenuMode.Pause) // TODO: add isGamePlaying from game manager? or a scene check?
            PauseGame();
        if (currentMenu != null) // should always be the case but oh well...
            currentMenu.Escape();
    }

    public void ToMainMenu()
    {
        GameManager.Instance.SaveGame();

        ToMenu(null, false);
        GameManager.Instance.sceneLoader.FullLoadSceneGroup(SceneGroup.MainMenu, () => SetMenuMode(MenuMode.Main));
    }

    public void SetMenuMode(MenuMode setMode)
    {
        mode = setMode;

        switch (mode)
        {
            case MenuMode.Main:
                // Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case MenuMode.Pause:
                // Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case MenuMode.None:
                Time.timeScale = 1f;
                // Cursor.lockState = CursorLockMode.Locked;
                // Cursor.visible = false;
                break;
        }
    }

    private void OnDestroy()
    {
        seq?.Kill();
        DOTween.KillAll();
    }
}

public enum MenuMode { Main, Pause, None }
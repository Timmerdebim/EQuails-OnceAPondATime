using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    private Menu currentMenu;
    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap playerInputActionMap;
    private InputActionMap uiInputActionMap;
    public bool isPaused { get; private set; } = false;

    private Sequence seq;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        playerInputActionMap = inputActions.FindActionMap("Player");
        uiInputActionMap = inputActions.FindActionMap("UI");
    }

    public void ToMenu(Menu toMenu, bool doFade)
    {
        if (doFade) MenuTransition(currentMenu, toMenu);
        else DirectMenuTransition(currentMenu, toMenu);
    }

    public void DirectMenuTransition(Menu fromMenu, Menu toMenu)
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
            EnableMenuMode(true);
            toMenu.gameObject.SetActive(true);
            toMenu.EnteringMenu();
            toMenu.EnteredMenu();
        }
        else
            EnableMenuMode(false);
    }

    public void MenuTransition(Menu fromMenu, Menu toMenu)
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
                EnableMenuMode(true);
                toMenu.gameObject.SetActive(true);
                toMenu.EnteringMenu();
            });
            seq.Append(toMenu.canvasGroup.DOFade(1, toMenu.fadeInDuration));
            seq.AppendCallback(() => { toMenu.EnteredMenu(); });
        }
        else
            seq.AppendCallback(() => { EnableMenuMode(false); });
    }

    public void PauseGame()
    {
        GameManager.Instance.sceneLoader.LoadScenesByGroup(SceneGroup.Pause);
        isPaused = true;
    }

    public void ResumeGame()
    {
        ToMenu(null, false);

        GameManager.Instance.sceneLoader.UnloadScenesByGroup(SceneGroup.Pause);
        isPaused = false;
    }

    public void Escape()
    {
        if (currentMenu == null && !isPaused) // TODO: add isGamePlaying from game manager? or a scene check?
            PauseGame();
        currentMenu.Escape();
    }

    public void ToMainMenu()
    {
        GameManager.Instance.SaveGame();

        ToMenu(null, false);
        Time.timeScale = 1f;
        isPaused = false;

        GameManager.Instance.sceneLoader.FullLoadSceneGroup(SceneGroup.MainMenu);
    }

    public void EnableMenuMode(bool isEnabled)
    {
        if (isEnabled)
        {
            // Time.timeScale = 0f;
            playerInputActionMap.Disable();
            uiInputActionMap.Enable();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Time.timeScale = 1f;
            playerInputActionMap.Enable();
            uiInputActionMap.Disable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }


    private void OnDestroy()
    {
        seq?.Kill();
        DOTween.KillAll();
    }
}
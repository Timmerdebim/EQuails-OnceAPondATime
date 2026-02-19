using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    private Menu currentMenu;
    [SerializeField] private Menu pauseMenu;

    private Sequence seq;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public void ToMenu(Menu toMenu, bool doFade)
    {
        MenuTransition(currentMenu, toMenu, doFade);
    }

    public void MenuTransition(Menu fromMenu, Menu toMenu, bool doFade)
    {
        seq?.Kill();
        seq = DOTween.Sequence();

        if (fromMenu != null)
        {
            seq.AppendCallback(() => { fromMenu.ExitingMenu(); });
            if (doFade) seq.Append(fromMenu.canvasGroup.DOFade(0, fromMenu.fadeOutDuration));
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
            if (doFade) seq.Append(toMenu.canvasGroup.DOFade(1, toMenu.fadeInDuration));
            seq.AppendCallback(() => { toMenu.EnteredMenu(); });
        }
    }

    public void PauseGame()
    {
        ToMenu(pauseMenu, false);
    }

    public void ResumeGame()
    {
        ToMenu(null, false);
    }

    public void Escape()
    {
        //if (pauseMenu != null && currentMenu == null)
        //    PauseGame();
        currentMenu.Escape();
    }

    private void OnDestroy()
    {
        seq?.Kill();
        DOTween.KillAll();
    }
}
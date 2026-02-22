using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Project.Menus
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance { get; private set; }
        private Menu currentMenu;
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

        public void Escape()
        {
            if (currentMenu != null)
                currentMenu.Escape();
        }

        private void OnDestroy()
        {
            seq?.Kill();
            DOTween.KillAll();
        }
    }
}
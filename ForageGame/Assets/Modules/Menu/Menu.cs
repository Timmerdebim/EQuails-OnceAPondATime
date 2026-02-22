using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Menus
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Menu : MonoBehaviour
    {
        [Header("Menu Settings")]
        [SerializeField] public CanvasGroup canvasGroup;
        [SerializeField] public GameObject firstSelected;
        [SerializeField] public float fadeInDuration = 0.5f;
        [SerializeField] public float fadeOutDuration = 0.5f;

        public virtual void Escape()
        {
            throw new NotImplementedException();
        }

        public virtual void EnteringMenu()
        {
            Debug.Log("Entering menu " + this);
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
        }

        public virtual void EnteredMenu()
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1;
            EventSystem.current.SetSelectedGameObject(firstSelected);
            Debug.Log("Entered menu " + this);
        }

        public virtual void ExitingMenu()
        {
            Debug.Log("Exiting menu " + this);
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.alpha = 1;
        }

        public virtual void ExitedMenu()
        {
            canvasGroup.alpha = 0;
            Debug.Log("Exited menu " + this);
        }
    }
}

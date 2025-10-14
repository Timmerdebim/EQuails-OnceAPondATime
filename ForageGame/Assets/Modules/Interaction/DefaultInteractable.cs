using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Modules.Interaction
{
    public class DefaultInteractable : MonoBehaviour, IInteractable
    {
        InteractablePrompt PopupPrompt; // UI element to prompt the player to interact

        public UnityEvent onInteract; // Event to invoke when interacting
        public UnityEvent onStopInteract; // Event to invoke when stopping interaction

        private void Start()
        {
            PopupPrompt = GetComponentInChildren<InteractablePrompt>(true);
        }

        public virtual void Focus()
        {
            print("Focused on " + gameObject.name);

            PopupPrompt?.Activate();
        }

        public virtual void Unfocus()
        {
            print("Unfocused from " + gameObject.name);

            PopupPrompt?.Deactivate();
        }

        public virtual void Interact(UnityAction StopInteractionCallback)
        {
            print("Interacting with " + gameObject.name);

            onInteract?.Invoke();
            GetComponentInChildren<Renderer>().material.color = Color.cyan;
        }

        public virtual void StopInteract()
        {
            print("Stopped interacting with " + gameObject.name);

            onStopInteract?.Invoke();
            GetComponentInChildren<Renderer>().material.color = Color.gray;
        }

    }
}
using UnityEngine;
using Assets.Modules.Interaction;
using UnityEngine.Events;

namespace NPC
{
    public class NpcTerminal : MonoBehaviour, IInteractable
    {
        //InteractablePrompt PopupPrompt; // UI element to prompt the player to interact

            public UnityEvent onInteract; // Event to invoke when interacting
            public UnityEvent onFocus;
            public UnityEvent OnUnfocus;

            private void Start()
            {
                //PopupPrompt = GetComponentInChildren<InteractablePrompt>(true);
            }

            public virtual void Interact()
            {
                print("Interacting with " + gameObject.name);

                onInteract?.Invoke();
                GetComponentInChildren<Renderer>().material.color = Color.cyan;
            }

            public virtual void Focus()
            {
                print("Focused on " + gameObject.name);

                onFocus?.Invoke();

                //PopupPrompt?.Activate();
            }

            public virtual void Unfocus()
            {
                print("Unfocused from " + gameObject.name);

                OnUnfocus?.Invoke();

                //PopupPrompt?.Deactivate();
            }
    }

}
using Assets.Modules.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class InteractionActuator : MonoBehaviour, IInteractable
{
    public UnityEvent OnInteract;
    public UnityEvent OnFocus;
    public UnityEvent OnUnfocus;
    public void Interact() => OnInteract.Invoke();
    public void Focus() => OnFocus.Invoke();
    public void Unfocus() => OnUnfocus.Invoke();
}

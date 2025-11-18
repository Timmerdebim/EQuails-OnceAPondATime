using UnityEngine;
using Assets.Modules.Interaction;
using UnityEngine.Events;

public class Pickup : IInteractable
{
    public void Focus()
    {
        throw new System.NotImplementedException();
        // TODO: Highlight
    }

    public void Interact(UnityAction StopInteractionCallback)
    {
        throw new System.NotImplementedException();
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }

    public void Unfocus()
    {
        throw new System.NotImplementedException();
        // TODO: de-Highlight
    }
}

using Assets.Modules.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class CraftingButton : MonoBehaviour, IInteractable
{
    [SerializeField] Crafter crafter;

    public void Focus()
    {
    }

    public void Interact(UnityAction StopInteractionCallback)
    {
        crafter.TryCraft();
    }

    public void StopInteract()
    {
    }

    public void Unfocus()
    {
    }
}

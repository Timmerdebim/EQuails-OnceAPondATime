using UnityEngine;
using Assets.Modules.Interaction;
using UnityEngine.Events;

public class WorldItem : MonoBehaviour, IInteractable
{
    public Item itemData;

    public UnityEvent onPickup;

    public void Interact(UnityAction StopInteractionCallback)
    {
        bool wasPickedUp = InventorySystem.Instance.PickupItem(itemData);

        if (wasPickedUp)
        {
            onPickup?.Invoke();
            Destroy(gameObject);
        }
    }

    public void StopInteract()
    {
        // None
    }

    public void Focus()
    {
        // Todo: Highlight
    }

    public void Unfocus()
    {
        // Todo: De-Highlight
    }
}
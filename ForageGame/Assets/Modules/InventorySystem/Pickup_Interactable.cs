using UnityEngine;
using Assets.Modules.Interaction;
using UnityEngine.Events;

[RequireComponent(typeof(Pickup))]
public class Pickup_Interactable : MonoBehaviour, IInteractable
{
    private Pickup pickup;

    void Start()
    {
        pickup = GetComponent<Pickup>();
    }

    public void Focus()
    {
        print(1);
        if (Inventory.Instance.isInventoryFull) return;      // Check if inventory space
        // TODO: Highlight
    }

    public void Interact(UnityAction StopInteractionCallback)
    {
        print(2);
        if (Inventory.Instance.isInventoryFull) return;      // Check if inventory space
        Inventory.Instance.PickupItem(pickup);               // Add to inventory
    }

    public void StopInteract()
    {
        // Nothing
    }

    public void Unfocus()
    {
        // TODO: de-Highlight
    }
}
using UnityEngine;
using Assets.Modules.Interaction;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class WorldItem : MonoBehaviour, IInteractable
{
    public Item item;
    private SpriteRenderer spriteRenderer;

    public UnityEvent onPickup;

    void Awake()
    {
        Initialize(item);
    }

    public void Initialize(Item data)
    {
        item = data;
        if (item != null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = item.GetSprite();
        }
    }


    #region  I Interactable

    virtual public void Interact(UnityAction StopInteractionCallback)
    {
        if (Inventory.Instance.hotbar.TryPickupItem(item))
        {
            // onPickup?.Invoke();
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

    #endregion
}
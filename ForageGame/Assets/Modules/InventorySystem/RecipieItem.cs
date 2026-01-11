using UnityEngine;
using Assets.Modules.Interaction;
using UnityEngine.Events;

public class RecipieItem : WorldItem
{
    public RecipeSO[] recipies;

    override public void Interact(UnityAction StopInteractionCallback)
    {
        // Unlock Recipies
        // TODO
        // TODO: show discovery
        Destroy(gameObject);
    }
}

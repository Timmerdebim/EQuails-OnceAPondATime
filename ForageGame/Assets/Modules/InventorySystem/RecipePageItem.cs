using UnityEngine;
using Assets.Modules.Interaction;
using UnityEngine.Events;
using System;


/// <summary>
/// Is part of the RecipeBook module, but I found this thing already here so uhh we ball
/// ~Lars
/// </summary>

public class RecipePageItem : WorldItem
{
    public RecipeSO[] recipies; //TODO: I don't actually unlock anything, this was already here. the RecipePageSO is just for the visual stuff

    public static event Action<RecipePageSO> onRecipePagePickup;

    [SerializeField] private RecipePageSO recipePageSO;

    override public void Interact(UnityAction StopInteractionCallback)
    {
        onRecipePagePickup?.Invoke(recipePageSO);
        // Unlock Recipies
        // TODO
        // TODO: show discovery
        Destroy(gameObject);
    }
}

using UnityEngine;
using Assets.Modules.Interaction;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Items/Recipe")]
public class RecipeItem : Item
{
    [Header("Recipe")]

    [SerializeField] protected List<Item> craftingIngredients;
    [SerializeField] protected Item craftingResult;
    [SerializeField] protected bool isRepeatCraftable;
    [SerializeField] private Sprite recipeVisualizationSprite;

    public List<Item> GetCraftingIngredients() => craftingIngredients;
    public Item GetCraftingResult() => craftingResult;
    public bool GetIsRepeatCraftable() => isRepeatCraftable;
    public Sprite GetRecipeVisualizationSprite() => recipeVisualizationSprite;

    public override bool Use()
    {
        if (isRepeatCraftable)
            return true;
        else
            return Inventory.Instance.recipeBook.TryRemoveRecipe(this);
    }

    public override bool TryPickup()
    {
        if (!Inventory.Instance.recipeBook.TryAddRecipe(this))
            return false;

        //TODO: add discovery animation
        return true;
    }
}

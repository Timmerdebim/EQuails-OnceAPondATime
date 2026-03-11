using UnityEngine;
using System.Collections.Generic;
using Project.Items.Inventory;

namespace Project.Items.Types
{
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

        public override bool TryWorldItemInteract()
        {
            if (!Inventory.Inventory.Instance.recipeBook.TryAddRecipe(this))
                return false;

            //TODO: add discovery animation
            return true;
        }

        public override bool TryUse()
        {
            if (isRepeatCraftable)
                return true;
            else
                return Inventory.Inventory.Instance.recipeBook.TryRemoveRecipe(this);
        }
    }
}
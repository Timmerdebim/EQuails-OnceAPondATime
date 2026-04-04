using UnityEngine;
using System.Collections.Generic;
using TDK.ItemSystem.Inventory;

namespace TDK.ItemSystem.Types
{
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Items/Recipe")]
    public class RecipeItem : ItemData
    {
        [Header("Recipe")]

        [SerializeField] protected List<ItemData> craftingIngredients;
        [SerializeField] protected ItemData craftingResult;
        [SerializeField] protected bool isRepeatCraftable;
        [SerializeField] private Sprite recipeVisualizationSprite;

        public List<ItemData> GetCraftingIngredients() => craftingIngredients;
        public ItemData GetCraftingResult() => craftingResult;
        public bool GetIsRepeatCraftable() => isRepeatCraftable;
        public Sprite GetRecipeVisualizationSprite() => recipeVisualizationSprite;

        public override bool TryWorldItemInteract()
        {
            if (!RecipeBookController.Instance.TryAddRecipe(this))
                return false;

            //TODO: add discovery animation
            return true;
        }

        public override bool TryUse()
        {
            if (isRepeatCraftable)
                return true;
            else
                return RecipeBookController.Instance.TryRemoveRecipe(this);
        }
    }
}
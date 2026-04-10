using DG.Tweening;
using TDK.ItemSystem.Types;
using UnityEngine;

namespace TDK.ItemSystem.Inventory
{
    public class Crafter : MonoBehaviour
    {
        [SerializeField] private ItemRackController _itemRack;
        [SerializeField] private SimpleItemSpawner _itemSpawner;
        [SerializeField] private Animator _animator;

        bool craftInProgress = false;

        public bool TryCraft()
        {
            if (craftInProgress) return false;
            foreach (RecipeItem recipeItem in RecipeBookController.Instance.CollectedRecipes)
            {
                if (TryCraftRecipe(recipeItem))
                    return true;
            }
            return false;
        }

        private bool TryCraftRecipe(RecipeItem recipeItem)
        {
            if (_itemRack.ContainsItemsExactly(recipeItem.GetCraftingIngredients()))
            {
                CraftRecipe(recipeItem);
                return true;
            }
            else return false;
        }

        private void CraftRecipe(RecipeItem recipeItem)
        {
            craftInProgress = true;
            // _itemRack.RemoveAll(); Animator Does This
            _itemSpawner.SetItem(recipeItem.GetCraftingResult());
            _animator.SetTrigger("Craft");
        }

        public void OnFinishCrafting()
        {
            craftInProgress = false;
        }
    }
}
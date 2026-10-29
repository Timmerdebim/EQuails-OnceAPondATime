using System.Collections.Generic;
using TDK.ItemSystem.Types;
using UnityEngine;
using FMODUnity;

namespace TDK.ItemSystem.Inventory
{
    public class Crafter : MonoBehaviour
    {
        [SerializeField] private ItemRackController _itemRack;
        [SerializeField] private SimpleItemSpawner _itemSpawner;
        [SerializeField] private Animator _animator;
        [SerializeField] private StoryFlag crafterUsedFlag;

        [Header("Particles")]
        [SerializeField] private ParticleSystem lidClosedParticles;
        [SerializeField] private ParticleSystem lidOpenedParticles;
        [SerializeField] private ParticleSystem fireParticles;
        [SerializeField] private ParticleSystem fireSmokeParticles;

        [Header("Sfx")]
        [SerializeField] private EventReference craftSfx;

        private bool craftInProgress = false;

        public void TryCraftVoid() => TryCraft();

        public bool TryCraft()
        {
            if (craftInProgress) return false;
            foreach (RecipeItem recipeItem in RecipeBookController.Instance.CollectedRecipes)
            {
                if (TryCraftRecipe(recipeItem))
                {
                    StoryFlagManager.Instance.AddFlag(crafterUsedFlag); //used only for the tutorial, to make the pot's dialogue disappear
                    return true;
                }
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
            if (!recipeItem.GetIsRepeatCraftable()) RecipeBookController.Instance?.TryRemoveRecipe(recipeItem);
            _itemSpawner.SetItem(recipeItem.GetCraftingResult());
            _animator.SetTrigger("Craft");
            RuntimeManager.PlayOneShot(craftSfx, transform.position);
        }

        public void AnimatorOnLidClose()
        {
            lidClosedParticles.Play();
            fireParticles.Play();
            fireSmokeParticles.Play();
            List<ItemController> itemControllers = new(_itemRack.GetItemControllers()); // Make a copy
            foreach (ItemController item in itemControllers)
                Destroy(item.gameObject);
        }

        public void AnimatorOnLidOpen()
        {
            lidOpenedParticles.Play();
            _itemSpawner.SpawnLocal();
            craftInProgress = false;
        }
    }
}
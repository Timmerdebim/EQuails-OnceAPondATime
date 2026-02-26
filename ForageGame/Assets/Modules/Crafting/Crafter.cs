using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Crafter : MonoBehaviour
{
    [SerializeField] ItemStand[] craftingSlots;

    bool craftInProgress = false;

    private void Update()
    {
        if (!craftInProgress) { TryCraft(); }

    }

    public void TryCraft()
    {
        List<Item> items = new List<Item>();
        foreach (ItemStand slot in craftingSlots)
        {
            if (slot.GetHeldItem() != null)
            {
                items.Add(slot.GetHeldItem().item);
            }
            else
            {
                // Slot is empty
                continue;
            }
        }

        foreach (RecipeItem recipeItem in Inventory.Instance.recipeBook.CollectedRecipes)
        {
            if (CanCraft(recipeItem, items))
            {
                Craft(recipeItem);
                break;
            }
        }
    }

    private void Craft(RecipeItem recipeItem)
    {
        // Assumes CanCraft has already been called and returned true

        craftInProgress = true;

        Sequence tweens = DOTween.Sequence();

        // Remove required items from slots
        foreach (Item requiredItem in recipeItem.GetCraftingIngredients())
        {
            foreach (ItemStand slot in craftingSlots)
            {
                if (slot.GetHeldItem() != null && slot.GetHeldItem().item == requiredItem)
                {
                    WorldItem item = slot.GetHeldItem();
                    slot.RemoveItem();
                    item.enabled = false; // Disable WorldItem component so that it can't be picked up or pulled on during animations

                    tweens.Insert(0, item.transform.DOMove(transform.position - 0.3f * (transform.position - item.transform.position).normalized, 0.5f).SetEase(Ease.InCubic));
                    // Add a short delay here before starting the scale (but don't wait for move to finish ?
                    tweens.Insert(0, item.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InExpo).OnComplete(() =>
                    {
                        Destroy(item.gameObject);
                    }));
                }
            }
        }


        // Spawn the result item at the crafter's position when tweens complete
        tweens.OnComplete(() =>
        {
            Inventory.Instance.SpawnItemAt(recipeItem.GetCraftingResult(), transform.position + Vector3.up);
            craftInProgress = false;
        });

    }

    private bool CanCraft(RecipeItem recipeItem, List<Item> itemsAvailable)
    {
        List<Item> itemsCopy = new List<Item>(itemsAvailable);
        foreach (Item requiredItem in recipeItem.GetCraftingIngredients())
        {
            bool found = false;
            for (int i = 0; i < itemsCopy.Count; i++)
            {
                if (itemsCopy[i] == requiredItem)
                {
                    found = true;
                    itemsCopy.RemoveAt(i);
                    break;
                }
            }
            if (!found)
            {
                return false;
            }
        }
        return true;
    }
}

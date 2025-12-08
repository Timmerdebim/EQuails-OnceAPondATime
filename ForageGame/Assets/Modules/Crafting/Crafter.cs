using System.Collections.Generic;
using UnityEngine;

public class Crafter : MonoBehaviour
{
    [SerializeField] ItemStand[] craftingSlots;
    [SerializeField] RecipeSO[] recipes;

    private void Update()
    {
        TryCraft();
    }

    public void TryCraft()
    {
        List<Item> items = new List<Item>();
        foreach (ItemStand slot in craftingSlots)
        {
            if (slot.GetHeldItem() != null)
            {
                items.Add(slot.GetHeldItem());
            }
            else
            {
                // Slot is empty
                continue;
            }
        }

        foreach (RecipeSO recipe in recipes)
        {
            if (CanCraft(recipe, items))
            {
                Craft(recipe);
            }
        }
    }

    private void Craft(RecipeSO recipe)
    {
        // Assumes CanCraft has already been called and returned true

        // Remove required items from slots
        foreach (Item requiredItem in recipe.requiredItems)
        {
            foreach (ItemStand slot in craftingSlots)
            {
                if (slot.GetHeldItem() == requiredItem)
                {
                    slot.RemoveItem();
                    break;
                }
            }
        }

        // Spawn the result item at the crafter's position
        Instantiate(recipe.resultItem.worldPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);

    }

    private bool CanCraft(RecipeSO recipe, List<Item> itemsAvailable)
    {
        List<Item> itemsCopy = new List<Item>(itemsAvailable);
        foreach (Item requiredItem in recipe.requiredItems)
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

using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Crafter : MonoBehaviour
{
    [SerializeField] ItemStand[] craftingSlots;
    [SerializeField] RecipeSO[] recipes;

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
                items.Add(slot.GetHeldItem().itemData);
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
                break;
            }
        }
    }

    private void Craft(RecipeSO recipe)
    {
        // Assumes CanCraft has already been called and returned true

        craftInProgress = true;

        Sequence tweens = DOTween.Sequence();

        // Remove required items from slots
        foreach (Item requiredItem in recipe.requiredItems)
        {
            foreach (ItemStand slot in craftingSlots)
            {
                if (slot.GetHeldItem().itemData == requiredItem)
                {
                    WorldItem item = slot.GetHeldItem();
                    slot.RemoveItem();
                    tweens.Insert(0, item.transform.DOMove(transform.position, 0.5f).SetEase(Ease.OutCubic));
                    tweens.Insert(1, item.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InCubic).OnComplete(() =>
                    {
                        Destroy(item.gameObject);
                    }));
                }
            }
        }


        // Spawn the result item at the crafter's position when tweens complete
        tweens.OnComplete(() => SpawnItem(recipe.resultItem));

    }

    private void SpawnItem(Item item)
    {
        GameObject spawnedItem = Instantiate(item.worldPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        spawnedItem.transform.localScale = Vector3.zero;
        spawnedItem.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InOutBack);

        craftInProgress = false;
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

using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Crafter : MonoBehaviour
{
    [SerializeField] ItemRack itemRack;
    private Sequence seq;
    [SerializeField] Transform transformLid;

    bool craftInProgress = false;

    void Awake()
    {
        transformLid.gameObject.SetActive(false);
    }

    public bool TryCraft()
    {
        if (craftInProgress) return false;
        foreach (RecipeItem recipeItem in Inventory.Instance.recipeBook.CollectedRecipes)
        {
            if (TryCraftRecipe(recipeItem))
                return true;
        }
        return false;
    }

    private bool TryCraftRecipe(RecipeItem recipeItem)
    {
        if (itemRack.ContainsItemsExactly(recipeItem.GetCraftingIngredients()))
        {
            CraftRecipe(recipeItem);
            return true;
        }
        else return false;
    }

    private void CraftRecipe(RecipeItem recipeItem)
    {
        craftInProgress = true;
        itemRack.gameObject.SetActive(false);

        seq?.Kill();
        seq = DOTween.Sequence();

        Vector3 initialLidPosition = transformLid.position;
        Quaternion initialLidRotation = transformLid.rotation;
        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;

        // Close the pot lid
        transformLid.gameObject.SetActive(true);
        seq.Append(transformLid.DOMove(transform.position + Vector3.up * 1, 0.5f).SetEase(Ease.InExpo));

        foreach (WorldItem worldItem in itemRack.GetWorldItems())
        {
            seq.Join(worldItem.transform
            .DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InExpo)
            .OnComplete(() => Destroy(worldItem.gameObject)));
        }

        seq.AppendCallback(() => itemRack.RemoveAll())
        .AppendInterval(0.5f)

        // Hop left
        .Append(transform.DOJump(initialPosition + Vector3.up * 1, 3, 1, 0.5f))
        .Join(transform.DOBlendableRotateBy(-15 * Vector3.forward, 0.7f).SetEase(Ease.OutBack))
        .AppendInterval(0.2f)

        // Hop right
        .Append(transform.DOJump(initialPosition + Vector3.up * 1, 3, 1, 0.5f))
        .Join(transform.DOBlendableRotateBy(30 * Vector3.forward, 0.7f).SetEase(Ease.OutBack))
        .AppendInterval(0.2f)

        // Hop left
        .Append(transform.DOJump(initialPosition + Vector3.up * 1, 3, 1, 0.5f))
        .Join(transform.DOBlendableRotateBy(-30 * Vector3.forward, 0.7f).SetEase(Ease.OutBack))
        .AppendInterval(0.2f)

        // Return to initial
        .Append(transform.DOJump(initialPosition, 3, 1, 0.5f))
        .Join(transform.DORotateQuaternion(initialRotation, 0.7f).SetEase(Ease.OutBack))
        .AppendInterval(0.5f)

        // Open lid
        .Append(transformLid.DOMove(initialLidPosition, 0.5f).SetEase(Ease.OutExpo));

        // Spawn item
        seq.AppendCallback(() =>
        {
            transformLid.SetPositionAndRotation(initialLidPosition, initialLidRotation);
            transform.SetPositionAndRotation(initialPosition, initialRotation);
            transformLid.gameObject.SetActive(false);
            Inventory.Instance.SpawnItemAt(recipeItem.GetCraftingResult(), transform.position + Vector3.up);
            itemRack.gameObject.SetActive(true);
            craftInProgress = false;
        });
    }
}

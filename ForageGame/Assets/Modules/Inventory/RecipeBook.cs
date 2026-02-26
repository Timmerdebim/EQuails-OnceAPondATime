using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using UnityEngine.UI;
using Unity.VisualScripting;
using System;

[RequireComponent(typeof(Animator))]
public class RecipeBook : MonoBehaviour
{
    [SerializeField] private List<RecipeItem> collectedRecipes;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public bool TryAddRecipe(RecipeItem recipeItem)
    {
        if (collectedRecipes.Contains(recipeItem))
            return false;
        collectedRecipes.Add(recipeItem);
        return true;
    }

    public bool TryRemoveRecipe(RecipeItem recipeItem)
    {
        if (collectedRecipes.Contains(recipeItem))
            return false;

        collectedRecipes.Remove(recipeItem);
        return true;
    }



    #region Triggers

    public void TriggerVisualization()
    {
        Debug.Log(5);
        IsVisualized = !IsVisualized;
        animator.SetBool("OpenRecipeBook", IsVisualized);
        if (IsVisualized)
            BuildStack();
        else
        {
            //TODO: wait for animation to finish
            DestroyStack();
        }
    }

    public void NextPage()
    {
        if (IsVisualized)
        {
            if (currentPageIndex + 1 >= pageObjects.Count) return;
            print($"flipping page {currentPageIndex} left");
            pageObjects[currentPageIndex].GetComponent<RecipePageUI>().PlayFlipLeftAnim(); //yes this sucks, I know shhhh
            currentPageIndex++;
        }
    }

    public void PreviousPage()
    {
        if (IsVisualized)
        {
            if (currentPageIndex < 1) return;
            currentPageIndex--;
            print($"flipping page {currentPageIndex} right");
            pageObjects[currentPageIndex].GetComponent<RecipePageUI>().PlayFlipRightAnim(); //yes this sucks, I know shhhh
        }
    }

    #endregion

    #region Visualization

    [Header("Visualization")]
    private List<GameObject> pageObjects = new List<GameObject>();
    [SerializeField] private GameObject pagePrefab;
    [SerializeField] private int currentPageIndex = 0;
    [SerializeField] private int xStackOffset = 1;
    public bool IsVisualized { get; private set; } = false;

    private void BuildStack()
    {
        for (int i = 0; i < collectedRecipes.Count; i++)
        {
            GameObject obj = Instantiate(pagePrefab, transform, false);

            //set the image sprite (its in the children because of shitty ui reasons)
            var image = obj.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
            image.sprite = collectedRecipes[i].GetRecipeVisualizationSprite();

            //position page UI (stacking offset)
            RectTransform imgRect = obj.transform.GetChild(0).GetComponent<RectTransform>();
            imgRect.anchoredPosition = new Vector2(xStackOffset * i, 0);

            pageObjects.Add(obj);
        }

        //Now set the draw order, because this is *of course* managed by hierarchy order
        //yes we must reverse it
        for (int i = 0; i < pageObjects.Count; i++)
        {
            pageObjects[i].transform.SetSiblingIndex(pageObjects.Count - 1 - i);
        }
    }

    private void DestroyStack()
    {
        currentPageIndex = 0;
        foreach (var page in pageObjects)
        {
            Destroy(page.gameObject);
        }
        pageObjects.Clear();
    }


    #endregion

    #region Save & Load

    public void SetData(List<int> recipeIds)
    {
        collectedRecipes = new List<RecipeItem>();

        foreach (int recipeId in recipeIds)
        {
            Item item = Inventory.Instance.GetItemById(recipeId);
            if (item is RecipeItem recipeItem)
                collectedRecipes.Add(recipeItem);
            else
                Debug.Log("Inventory: Critical error in retrieving recipes by ID");
        }
    }

    public List<int> GetData()
    {
        List<int> recipeIds = new List<int>();

        foreach (var recipeItem in collectedRecipes)
            recipeIds.Add(Inventory.Instance.GetIdByItem(recipeItem));

        return recipeIds;
    }

    #endregion
}

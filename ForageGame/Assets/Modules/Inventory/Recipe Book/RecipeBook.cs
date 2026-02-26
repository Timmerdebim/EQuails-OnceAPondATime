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
    public List<RecipeItem> CollectedRecipes { get; set; } = new();


    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public bool TryAddRecipe(RecipeItem recipeItem)
    {
        if (CollectedRecipes.Contains(recipeItem))
            return false;
        SetVisualization(false);
        CollectedRecipes.Add(recipeItem);
        return true;
    }

    public bool TryRemoveRecipe(RecipeItem recipeItem)
    {
        if (CollectedRecipes.Contains(recipeItem))
            return false;
        SetVisualization(false);
        CollectedRecipes.Remove(recipeItem);
        return true;
    }

    #region Triggers

    public void TriggerVisualization()
    {
        SetVisualization(!IsVisualized);
    }

    public void SetVisualization(bool isEnabled)
    {
        if (CollectedRecipes.Count == 0)
            IsVisualized = false;
        else IsVisualized = isEnabled;
        animator.SetBool("OpenRecipeBook", IsVisualized);
        DestroyStack();
        if (IsVisualized) BuildStack();
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
        for (int i = 0; i < CollectedRecipes.Count; i++)
        {
            GameObject obj = Instantiate(pagePrefab, transform, false);

            //set the image sprite (its in the children because of shitty ui reasons)
            var image = obj.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
            image.sprite = CollectedRecipes[i].GetRecipeVisualizationSprite();

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
        CollectedRecipes = new List<RecipeItem>();

        foreach (int recipeId in recipeIds)
        {
            Item item = Inventory.Instance.GetItemById(recipeId);
            if (item is RecipeItem recipeItem)
                CollectedRecipes.Add(recipeItem);
            else
                Debug.Log("Inventory: Critical error in retrieving recipes by ID");
        }
    }

    public List<int> GetData()
    {
        List<int> recipeIds = new List<int>();

        foreach (var recipeItem in CollectedRecipes)
            recipeIds.Add(Inventory.Instance.GetIdByItem(recipeItem));

        return recipeIds;
    }

    #endregion
}

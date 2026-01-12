using UnityEngine;
using System.Collections.Generic;

public class RecipeBookController : MonoBehaviour
{
    [SerializeField] private GameObject pagePrefab;
    [SerializeField] private List<RecipePageSO> collectedPages;
    private List<GameObject> pageObjects;
    private int currentPageIndex;

    void Awake()
    {
        RecipePageItem.onRecipePagePickup += OnRecipePageCollected;
    }

    private void OnRecipePageCollected(RecipePageSO page)
    {
        print($"New recipe collected: {page.Name}");
        collectedPages.Add(page);
    }

    public void OpenRecipeBook()
    {
        
    }
    public void CloseRecipeBook()
    {
        
    }

    public void NextPage()
    {
        
    }
    public void PreviousPage()
    {
        
    }
}

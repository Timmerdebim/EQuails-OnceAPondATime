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
        //listen in to the event
    }

    private void OnRecipePageCollected(RecipePageSO page)
    {
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

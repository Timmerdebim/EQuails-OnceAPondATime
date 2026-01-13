using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using UnityEngine.UI;

public class RecipeBookController : MonoBehaviour
{
    [SerializeField] private GameObject pagePrefab;
    [SerializeField] private List<RecipePageSO> collectedPages;
    private List<GameObject> pageObjects = new List<GameObject>();
    [SerializeField] private int currentPageIndex = 0;

    [SerializeField] private bool opened;

    private Animator animator;

    [SerializeField] private int xStackOffset = 1;

    void Awake()
    {
        RecipePageItem.onRecipePagePickup += OnRecipePageCollected;
        animator = GetComponent<Animator>();
    }

    private void OnRecipePageCollected(RecipePageSO page)
    {
        print($"New recipe collected: {page.Name}");
        collectedPages.Add(page);
    }


    //All of this is messy and preliminary, I want to get the system working, the specifics of how things are displayed are tbd
    void Update()
    {
        //TODO: this input here is placeholder for now just to get it to work ~Lars
        //Also TODO: lock player controls while menu open
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            opened = !opened;
            animator.SetBool("OpenRecipeBook", opened);
            if(opened)
            {
                BuildStack();
            }
            else
            {
                //TODO: wait for animation to finish
                DestroyStack();
            }
        }

        if(opened)
        {
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PreviousPage();
            }
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                NextPage();
            }
        }
    }

    private void BuildStack()
    {
        for (int i = 0; i < collectedPages.Count; i++)
        {
            GameObject obj = Instantiate(pagePrefab, transform, false);

            //set the image sprite (its in the children because of shitty ui reasons)
            var image = obj.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
            image.sprite = collectedPages[i].pageSprite;

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


    public void NextPage()
    {
        if(currentPageIndex+1 >= pageObjects.Count) return;
        print($"flipping page {currentPageIndex} left");
        pageObjects[currentPageIndex].GetComponent<RecipePageUI>().PlayFlipLeftAnim(); //yes this sucks, I know shhhh
        currentPageIndex++;
    }
    public void PreviousPage()
    {
        if(currentPageIndex < 1) return;
        currentPageIndex--;
        print($"flipping page {currentPageIndex} right");
        pageObjects[currentPageIndex].GetComponent<RecipePageUI>().PlayFlipRightAnim(); //yes this sucks, I know shhhh
    }
}

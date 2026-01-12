using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using UnityEngine.UI;

public class RecipeBookController : MonoBehaviour
{
    [SerializeField] private RectTransform pagePrefab;
    [SerializeField] private List<RecipePageSO> collectedPages;
    private List<RectTransform> pageObjects = new List<RectTransform>();
    private int currentPageIndex;

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


    void Update()
    {
        //TODO: this input here is placeholder for now just to get it to work ~Lars
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
    }

    private void BuildStack()
    {
        for (int i = 0; i < collectedPages.Count; i++)
        {
            var page = collectedPages[i];
            RectTransform obj = Instantiate(pagePrefab, transform);
            obj.GetComponent<UnityEngine.UI.Image>().sprite = page.pageSprite;
            obj.anchoredPosition = new Vector2(obj.anchoredPosition.x - xStackOffset*i, obj.anchoredPosition.y);
            pageObjects.Add(obj);
        }

    }

    private void DestroyStack()
    {
        foreach (var page in pageObjects)
        {
            Destroy(page.gameObject);
        }
        pageObjects.Clear();
    }


    //TODO: implement
    public void NextPage()
    {
        
    }
    public void PreviousPage()
    {
        
    }
}

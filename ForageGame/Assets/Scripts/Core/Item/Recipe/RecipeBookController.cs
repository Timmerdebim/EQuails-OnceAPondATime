using UnityEngine;
using System.Collections.Generic;
using TDK.ItemSystem.Types;
using TDK.SaveSystem;
using System.Linq;
using System;

namespace TDK.ItemSystem.Inventory
{
    [RequireComponent(typeof(Animator))]
    public class RecipeBookController : MonoBehaviour, ISaveable, ILoadable
    {
        public static RecipeBookController Instance;

        public List<RecipeItem> CollectedRecipes { get; set; } = new();
        public List<RecipeItem> UsedRecipes { get; set; } = new();

        private Animator animator;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;

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

        private List<RecipeItem> ItemsToRecipes(IEnumerable<ItemData> items)
        {
            List<RecipeItem> recipes = new();
            foreach (ItemData item in items)
            {
                if (item is RecipeItem recipe)
                    recipes.Add(recipe);
                else
                    Debug.LogWarning("Items: Cannot extract recipe from item.");
            }
            return recipes;
        }

        public void LoadData(WorldSaveData data)
        {
            CollectedRecipes = ItemsToRecipes(ItemServices.Instance.Database.GetAssets(data.Inventory.CollectedRecipes));
            UsedRecipes = ItemsToRecipes(ItemServices.Instance.Database.GetAssets(data.Inventory.CollectedRecipes));
        }

        public void SaveData(ref WorldSaveData data)
        {
            data.Inventory.CollectedRecipes = ItemServices.Instance.Database.GetIds(CollectedRecipes).ToList();
            data.Inventory.UsedRecipes = ItemServices.Instance.Database.GetIds(UsedRecipes).ToList();
        }

        #endregion
    }
}
using UnityEngine;
using System;
using System.Linq;
using DG.Tweening;

namespace Project.Items.Inventory
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance { get; private set; }
        [SerializeField] public RecipeBook recipeBook;
        [SerializeField] public Hotbar hotbar;
        [SerializeField] public ItemPickupUI itemPickupUI;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Next()
        {
            if (recipeBook.IsVisualized)
                recipeBook.NextPage();
            else
                hotbar.SelectNext();
        }

        public void Previous()
        {
            if (recipeBook.IsVisualized)
                recipeBook.PreviousPage();
            else
                hotbar.SelectPrevious();
        }

        #region Save & Load

        public void LoadData(InventoryData data)
        {
            hotbar.SetData(data.hotbarData);
            recipeBook.SetData(data.recipeBookData);
        }

        public void SaveData(ref InventoryData data)
        {
            data = new InventoryData
            {
                hotbarData = hotbar.GetData(),
                recipeBookData = recipeBook.GetData()
            };
        }

        #endregion
    }
}
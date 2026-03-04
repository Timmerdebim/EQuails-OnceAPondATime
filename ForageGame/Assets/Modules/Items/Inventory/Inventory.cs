using UnityEngine;
using System;
using System.Linq;
using DG.Tweening;
using System.Collections.Generic;

namespace Project.Items.Inventory
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance { get; private set; }
        [SerializeField] public RecipeBook recipeBook;
        [SerializeField] public Hotbar hotbar;
        [SerializeField] public ItemPickupUI itemPickupUI;

        public HashSet<Item> seenItems = new();

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

            seenItems = new();
            foreach (int itemId in data.seenItemsData)
                seenItems.Add(ItemManager.Instance.GetItemById(itemId));
        }

        public void SaveData(ref InventoryData data)
        {
            HashSet<int> seenItemsData = new();
            foreach (Item item in seenItems)
                seenItemsData.Add(ItemManager.Instance.GetIdByItem(item));

            data = new InventoryData
            {
                hotbarData = hotbar.GetData(),
                recipeBookData = recipeBook.GetData(),
                seenItemsData = seenItemsData
            };
        }

        #endregion
    }
}
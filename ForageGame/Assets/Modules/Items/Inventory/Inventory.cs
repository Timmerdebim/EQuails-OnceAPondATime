using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using DG.Tweening;

public class Inventory : MonoBehaviour
{
    [SerializeField] public RecipeBook recipeBook;
    [SerializeField] public Hotbar hotbar;
    [SerializeField] public Chest chest;
    [SerializeField] public ItemPickupPopup itemPickupPopup;
    [SerializeField] private GameObject worldItemPrefab;
    [SerializeField] private Item[] itemDatabase;

    public static Inventory Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public int GetIdByItem(Item item)
    {
        return Array.FindIndex(itemDatabase, row => row == item); // Returns -1 if not in database
    }

    public Item GetItemById(int id)
    {
        if (id < 0 || id >= itemDatabase.Count())
            return null;
        return itemDatabase[id];
    }

    public void SpawnItemAt(Item item, Vector3 position)
    {
        GameObject worldItem = Instantiate(worldItemPrefab, position, Quaternion.identity);
        worldItem.GetComponent<WorldItem>().Initialize(item);

        worldItem.transform.localScale = Vector3.zero;
        worldItem.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InOutBack);
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
        chest.SetData(data.chestData);
        recipeBook.SetData(data.recipeBookData);
    }

    public void SaveData(ref InventoryData data)
    {
        data = new InventoryData
        {
            hotbarData = hotbar.GetData(),
            chestData = chest.GetData(),
            recipeBookData = recipeBook.GetData()
        };
    }

    #endregion
}
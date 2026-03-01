using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using System;

// IMPORTANT: ItemRacks cannot overlap; this will result in breaking possibly everything!

public enum ItemRackAlignment { Left, Right, Center, Justified }

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(SplineContainer))]
public class ItemRack : MonoBehaviour
{
    [SerializeField] private ItemRackAlignment alignment = ItemRackAlignment.Left;
    [SerializeField] private float suckDuration = 1f;

    [SerializeField] private List<WorldItem> WorldItems = new();
    private SplineContainer splineContainer;

    void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();
    }

    void Update()
    {
        if (WorldItems.Contains(null)) // i need to think of a better solution to this (on trigger exit wont work since it is not triggered by Destroy)
            RefreshWorldItems();
    }

    void OnTriggerEnter(Collider other)
    {
        WorldItem worldItem = other.gameObject.GetComponent<WorldItem>();
        if (worldItem)
            AddWorldItem(worldItem);
    }

    public void RefreshWorldItems()
    {
        WorldItems.RemoveAll(item => item == null);
        UpdateWorldItemPositions();
    }

    #region Set Spline Position

    public void UpdateWorldItemPositions()
    {
        float dt = 1f / Mathf.Max(1, WorldItems.Count);

        for (int i = 0; i < WorldItems.Count; i++)
        {
            Vector3 target = Vector3.zero;
            switch (alignment)
            {
                case ItemRackAlignment.Left:
                    target = splineContainer.EvaluatePosition(dt * i);
                    break;
                case ItemRackAlignment.Right:
                    target = splineContainer.EvaluatePosition(dt * (i + 1));
                    break;
                case ItemRackAlignment.Center:
                    target = splineContainer.EvaluatePosition(dt * i + dt / 2);
                    break;
                case ItemRackAlignment.Justified:
                    target = splineContainer.EvaluatePosition(1 / (1 / dt - 1) * i);
                    break;
            }
            WorldItems[i]?.MoveTo(target, suckDuration);
        }
    }

    #endregion

    #region Getting & Setting

    public List<WorldItem> GetWorldItems() => new(WorldItems);  // RETURNS A COPY

    public List<Item> GetItems() // RETURNS A COPY
    {
        List<Item> items = new();
        foreach (WorldItem worldItem in GetWorldItems())
            items?.Add(worldItem.item);
        return items;
    }

    public bool ContainsItem(Item item) => GetItems().Contains(item);

    public bool ContainsItems(List<Item> items)
    {
        List<Item> itemsCopy = GetItems();
        foreach (Item item in items)
        {
            if (!itemsCopy.Remove(item))
                return false;
        }
        return true;
    }

    public bool ContainsItemsExactly(List<Item> items)
    {
        if (items.Count != GetItems().Count)
            return false;
        return ContainsItems(items);
    }

    public void AddWorldItem(WorldItem worldItem)
    {
        if (WorldItems.Contains(worldItem))
            return;
        WorldItems.Add(worldItem);
        RefreshWorldItems();
    }

    public void RemoveWorldItem(WorldItem worldItem)
    {
        if (!WorldItems.Contains(worldItem))
            return;
        WorldItems.Remove(worldItem);
        RefreshWorldItems();
    }

    public void RemoveItem(Item item)
    {
        foreach (WorldItem worldItem in WorldItems)
        {
            if (worldItem.item == item)
            {
                RemoveWorldItem(worldItem);
                return;
            }
        }
    }

    public void RemoveItems(List<Item> items)
    {
        foreach (Item item in items)
            RemoveItem(item);
    }

    public void RemoveAll()
    {
        WorldItems = new();
        RefreshWorldItems();
    }

    #endregion
}

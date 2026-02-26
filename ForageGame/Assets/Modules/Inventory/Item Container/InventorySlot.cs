using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using NUnit.Framework;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemQuantity;
    [SerializeField] private Image slotImage;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color notSelectedColor;

    public Item Item { get; private set; } = null;
    public int Quantity { get; private set; } = 0;

    public void Initialize()
    {
        SetItem(null, 0);
        SetSelected(false);
    }

    private void SetItem(Item item, int quantity)
    {
        if (item == null || quantity <= 0)
        {
            Item = null;
            Quantity = 0;
        }
        else
        {
            Item = item;
            Quantity = Math.Max(0, quantity);
        }
        RefreshVisuals();
    }

    public bool IsEmpty() => Item == null || Quantity <= 0;
    public bool ContainsItem(Item item) => Item != null && Item == item;

    public bool TryAddQuantity(int quantity)
    {
        if (quantity < 0)
            return false;
        if (Item == null)
            return false;

        SetItem(Item, Quantity + quantity);
        return true;
    }

    public bool TryAddItem(Item item, int quantity)
    {
        if (item == null || quantity < 0) return false;

        if (IsEmpty())
        {
            SetItem(item, quantity);
            return true;
        }
        if (ContainsItem(item))
            return TryAddQuantity(quantity);

        return false;
    }

    public bool TryRemoveQuantity(int quantity)
    {
        if (quantity < 0)
            return false;
        if (Item == null)
            return false;
        if (Quantity - quantity < 0)
            return false;

        SetItem(Item, Quantity - quantity);
        return true;
    }

    public bool TryRemoveItem(Item item, int quantity)
    {
        if (item == null || quantity < 0) return false;

        if (IsEmpty())
            return false;
        if (ContainsItem(item))
            return TryRemoveQuantity(quantity);

        return false;
    }

    #region Visuals

    public void RefreshVisuals()
    {
        if (IsEmpty())
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
            itemQuantity.text = "";
            itemQuantity.enabled = false;
        }
        else
        {
            itemImage.sprite = Item.GetSprite();
            itemImage.enabled = true;
            itemQuantity.text = Quantity.ToString();
            itemQuantity.enabled = true;
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
            slotImage.color = selectedColor;
        else
            slotImage.color = notSelectedColor;
    }

    #endregion

    #region Save & Load

    public InventorySlotData GetData()
    {
        if (IsEmpty())
        {
            return new InventorySlotData
            {
                itemId = -1,
                itemQuantity = 0
            };
        }
        return new InventorySlotData
        {
            itemId = Item.GetId(),
            itemQuantity = Quantity
        };
    }

    public void SetData(InventorySlotData inventorySlotData)
    {
        Item item = Inventory.Instance.GetItemById(inventorySlotData.itemId);
        SetItem(item, inventorySlotData.itemQuantity);
    }

    #endregion
}
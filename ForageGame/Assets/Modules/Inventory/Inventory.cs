using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Inventory : MonoBehaviour
{
    [Header("Hotbar Settings")]
    public int hotbarSize = 4;
    public Transform hotbarParent;
    public GameObject slotPrefab;

    [Header("Item Database")]
    public Item[] itemDatabase;

    private Item[] hotbarItems;
    private InventorySlot[] hotbarSlots;
    private int selectedSlot = 0;

    public static Inventory Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeHotbar();
    }

    void InitializeHotbar()
    {
        hotbarItems = new Item[hotbarSize];
        hotbarSlots = new InventorySlot[hotbarSize];

        // Clear existing slots
        foreach (Transform child in hotbarParent) Destroy(child.gameObject);

        // Create new slots
        for (int i = 0; i < hotbarSize; i++)
        {
            GameObject slotObject = Instantiate(slotPrefab, hotbarParent);
            InventorySlot slot = slotObject.GetComponent<InventorySlot>();
            slot.Initialize(i);
            hotbarSlots[i] = slot;
        }

        SelectSlot(0);
    }

    public bool PickupItem(Item item)
    {
        // Find first empty slot
        for (int i = 0; i < hotbarSize; i++)
        {
            if (hotbarItems[i] == null)
            {
                SetItem(i, item);
                // TODO: first time pickup screen
                StartCoroutine(ItemPickupPopup.Instance.ShowPopup(
                item.icon,
                item.itemName,
                item.description
            ));
                return true;
            }
        }

        Debug.Log("Hotbar is full!");
        return false;
    }

    public void DropItem()
    {
        if (selectedSlot < 0 || selectedSlot >= hotbarSize) return;
        if (hotbarItems[selectedSlot] == null) return;

        Item itemToDrop = hotbarItems[selectedSlot];

        // Spawn the item in the world
        if (itemToDrop.worldPrefab != null)
        {
            // Get player position
            Vector3 dropPosition = Player.Instance.transform.position; // TODO: move down to ground
            Instantiate(itemToDrop.worldPrefab, dropPosition, Quaternion.identity);
        }

        // Remove from hotbar
        RemoveItem(selectedSlot);
    }

    public void ConsumeItem()
    {
        if (selectedSlot < 0 || selectedSlot >= hotbarSize) return;
        if (hotbarItems[selectedSlot] == null) return;
        if (hotbarItems[selectedSlot] is not ConsumableItem item) return;

        // Get player position
        Player.Instance.energy.TakeDamage(-item.consumableEnergy); // a little botched but ok...
        // Remove from hotbar
        RemoveItem(selectedSlot);
        // Replace with new item
        if (item.returnItem == null) return;
        SetItem(selectedSlot, item.returnItem);
    }

    public bool GiveItem(Item item)
    {
        for (int i = 0; i < hotbarSize; i++)
        {
            if (item == hotbarItems[i])
            {
                RemoveItem(i);
                return true;
            }
        }
        return false;
    }

    public void RemoveItem(int index)
    {
        hotbarItems[index] = null;
        hotbarSlots[index].ClearSlot();
    }

    public void SetItem(int index, Item item)
    {
        hotbarItems[index] = item;
        hotbarSlots[index].SetItem(item);
    }

    public void SelectSlot(int index)
    {
        selectedSlot = index;

        for (int i = 0; i < hotbarSize; i++)
            hotbarSlots[i].SetSelected(i == index);
    }

    public Item GetItemById(string itemName)
    {
        foreach (Item item in itemDatabase)
        {
            if (item.itemName == itemName)
                return item;
        }
        return null;
    }
}
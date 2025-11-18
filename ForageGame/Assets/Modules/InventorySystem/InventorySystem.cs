using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    [Header("Hotbar Settings")]
    public int hotbarSize = 4;
    public Transform hotbarParent;
    public GameObject slotPrefab;

    [Header("Item Database")]
    public Item[] itemDatabase;

    private Item[] hotbarItems;
    private InventorySlot[] hotbarSlots;

    public static InventorySystem Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
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
            slot.Initialize(i, this);
            hotbarSlots[i] = slot;
        }
    }

    public bool PickupItem(Item item)
    {
        // Find first empty slot
        for (int i = 0; i < hotbarSize; i++)
        {
            if (hotbarItems[i] == null)
            {
                hotbarItems[i] = item;
                hotbarSlots[i].SetItem(item);
                return true;
            }
        }

        Debug.Log("Hotbar is full!");
        return false;
    }

    public void DropItem(int slotIndex, Vector3 dropPosition)
    {
        if (slotIndex < 0 || slotIndex >= hotbarSize) return;
        if (hotbarItems[slotIndex] == null) return;

        Item itemToDrop = hotbarItems[slotIndex];

        // Spawn the item in the world
        if (itemToDrop.worldPrefab != null)
        {
            Instantiate(itemToDrop.worldPrefab, dropPosition, Quaternion.identity);
        }

        // Remove from hotbar
        hotbarItems[slotIndex] = null;
        hotbarSlots[slotIndex].ClearSlot();
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
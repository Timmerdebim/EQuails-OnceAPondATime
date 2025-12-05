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

    [Header("Other")]
    public Transform player;
    public DuckEnergy duckEnergy;

    private Item[] hotbarItems;
    private InventorySlot[] hotbarSlots;
    private int selectedSlot = 0;

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

        SelectSlot(0);
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

    public void DropItem()
    {
        if (selectedSlot < 0 || selectedSlot >= hotbarSize) return;
        if (hotbarItems[selectedSlot] == null) return;

        Item itemToDrop = hotbarItems[selectedSlot];

        // Spawn the item in the world
        if (itemToDrop.worldPrefab != null)
        {
            // Get player position
            Vector3 dropPosition = player.position; // TODO: move down to ground
            Instantiate(itemToDrop.worldPrefab, dropPosition, Quaternion.identity);
        }

        // Remove from hotbar
        RemoveItem(selectedSlot);
    }

    public void ConsumeItem()
    {
        if (selectedSlot < 0 || selectedSlot >= hotbarSize) return;
        if (hotbarItems[selectedSlot] == null) return;

        Item itemToConsume = hotbarItems[selectedSlot];

        // Spawn the item in the world
        if (itemToConsume.isConsumable)
        {
            // Get player position
            duckEnergy.AddEnergy(itemToConsume.consumableEnergy);
            // Remove from hotbar
            RemoveItem(selectedSlot);
        }
        else Debug.Log("Item cannot be consumed");
    }

    public void RemoveItem(int index)
    {
        hotbarItems[index] = null;
        hotbarSlots[index].ClearSlot();
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
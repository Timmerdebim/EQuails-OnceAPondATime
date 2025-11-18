using System;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static Inventory _instance;
    public static Inventory Instance
    {
        get
        {
            if (_instance == null)
                Debug.Log("Inventory is null");
            return _instance;
        }
    }
    void Awake()
    {
        _instance = this;
    }

    [SerializeField] private InventorySlot[] inventorySlots;
    [SerializeField] public Canvas canvas;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private GameObject player;
    public bool isInventoryFull { get; private set; }

    public void PickupItem(Pickup pickup)
    {
        Debug.Log(3);
        if (pickup.isBusy) return;
        // We find the first empty inventory slot 
        foreach (InventorySlot inventorySlot in inventorySlots)
        {
            if (inventorySlot.pickupItem == null)
            {
                Debug.Log(4);
                inventorySlot.pickupItem = pickup;
                // We now set the target
                Vector2 targetScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, inventorySlot.rectTransform.position);
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, targetScreenPos, canvas.worldCamera, out localPoint);
                Vector3 targetPosition = canvasRect.TransformPoint(localPoint);
                pickup.PickupItem(targetPosition);
                isInventoryFull = QInventoryFull(); // re-check inventory state
            }
        }
        Debug.Log("Error: tried picking up item when inventory was full.");
        return;
    }

    public void ConsumeItem(InventorySlot inventorySlot)
    {
        Pickup pickup = inventorySlot.pickupItem;
        if (pickup == null) return;
        if (pickup.isBusy) return;
        if (pickup.isConsumable == false) return;
        // Else we now consume the item
        inventorySlot.pickupItem = null;
        pickup.ConsumeItem();
        isInventoryFull = false; // re-check inventory state
    }

    public void TossItem(InventorySlot inventorySlot)
    {
        Pickup pickup = inventorySlot.pickupItem;
        if (pickup == null) return;
        if (pickup.isBusy) return;
        // Else we now toss the item
        inventorySlot.pickupItem = null;
        pickup.TossItem(player.transform.position);     // TODO: offset infront of the player?
        isInventoryFull = false; // re-check inventory state
    }

    private bool QInventoryFull()
    {
        foreach (InventorySlot inventorySlot in inventorySlots)
        {
            if (inventorySlot.pickupItem == null) return false;
        }
        return true;
    }
}
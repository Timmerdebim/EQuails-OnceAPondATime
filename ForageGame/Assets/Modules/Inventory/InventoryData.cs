using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InventoryData
{
    public List<InventorySlotData> hotbarData = new();
    public List<InventorySlotData> chestData = new();
    public List<int> recipeBookData = new();
}
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InventoryData
{
    public ItemContainerData hotbarData = new();
    public ItemContainerData chestData = new();
}
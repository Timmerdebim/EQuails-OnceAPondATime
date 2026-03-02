using System.Collections.Generic;

namespace Project.Items.Inventory
{
    [System.Serializable]
    public class InventoryData
    {
        public List<InventorySlotData> hotbarData = new();
        public List<int> recipeBookData = new();
    }
}
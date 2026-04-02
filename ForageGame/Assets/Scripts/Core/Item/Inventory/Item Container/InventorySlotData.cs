namespace TDK.ItemSystem.Inventory
{
    [System.Serializable]
    public class ItemSlotSaveData
    {
        public string ItemId = null;
        public int ItemQuantity = new();

        public ItemData GetItemData() => ItemServices.Instance.Database.GetAsset(ItemId);
    }
}
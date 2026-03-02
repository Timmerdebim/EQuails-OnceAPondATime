using Project.Items.Inventory;

[System.Serializable]
public class SaveData
{
    public int playtimeSeconds = 0;
    public PlayerData playerData = new();
    public InventoryData inventoryData = new();
    // Story Flags (Abilities) (Recipies)
}
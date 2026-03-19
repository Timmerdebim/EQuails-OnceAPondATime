using Project.Items.Inventory;
using Project.Menus.Audio;
using Project.Menus.Graphics;
using Project.Menus.Keybind;

namespace TDK.SaveSystem
{
    /// <summary>
    /// SaveData represents the data of one save file.
    /// </summary>
    [System.Serializable]
    public class SaveData
    {

        public int playtimeSeconds = 0;
        public PlayerData playerData = new();
        public InventoryData inventoryData = new();

        public StoryData storyData = new();
    }
}
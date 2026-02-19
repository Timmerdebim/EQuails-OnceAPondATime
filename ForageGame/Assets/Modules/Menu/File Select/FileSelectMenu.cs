using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class FileSelectMenu : Menu
{
    [Header("UI References")]
    [SerializeField] private SaveSlotUI[] saveSlots = new SaveSlotUI[3];

    [Header("Connected Menus")]
    [SerializeField] private Menu mainMenu;

    [Header("Settings")]
    [SerializeField] private string savePath = "Assets/SaveData";

    private const string SAVE_FILE_FORMAT = "save_{0}.dat";

    private void RefreshMenu()
    {
        for (int i = 0; i < saveSlots.Length; i++)
        {
            string savePath = GetSavePath(i);

            if (File.Exists(savePath))
            {
                SaveData data = GameManager.Instance.LoadSaveData(i);
                saveSlots[i].slotText.text = "continue"; // TODO: add duck progress image?
                saveSlots[i].playtimeText.text = FormatPlaytime(data.playtimeSeconds);
                saveSlots[i].deleteButton.gameObject.SetActive(true);
            }
            else
            {
                saveSlots[i].slotText.text = "New Game";
                saveSlots[i].playtimeText.text = "";
                saveSlots[i].deleteButton.gameObject.SetActive(false);
            }
        }
    }

    public override void Escape()
    {
        MenuManager.Instance.ToMenu(mainMenu, true);
    }

    // ------------ Buttons ------------

    public void OnSlotSelected(int slotIndex)
    {
        string savePath = GetSavePath(slotIndex);

        if (File.Exists(savePath))
            GameManager.Instance.LoadGame(slotIndex);   // Load game with this save file
        else
            GameManager.Instance.NewGame(slotIndex);    // Create new game in this slot

        MenuManager.Instance.ToMenu(mainMenu, true); // TODO
    }

    public void OnDeleteSlot(int slotIndex)
    {
        string savePath = GetSavePath(slotIndex);
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            RefreshMenu();
        }
    }

    // ------------ Functions ------------

    private string GetSavePath(int slotIndex)
    {
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        return Path.Combine(savePath, string.Format(SAVE_FILE_FORMAT, slotIndex));
    }

    private string FormatPlaytime(float seconds)
    {
        int hours = (int)(seconds / 3600);
        int minutes = (int)((seconds % 3600) / 60);
        return $"{hours}h {minutes}m";
    }
}
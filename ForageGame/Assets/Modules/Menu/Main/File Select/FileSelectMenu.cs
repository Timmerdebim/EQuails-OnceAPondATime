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

    public override void EnteringMenu()
    {
        base.EnteringMenu();
        RefreshVisuals();
    }

    public override void Escape()
    {
        MenuManager.Instance.ToMenu(mainMenu, true);
    }

    // ------------ Buttons ------------

    // ------------ Functions ------------

    private void RefreshVisuals()
    {
        foreach (SaveSlotUI saveSlot in saveSlots)
            saveSlot.Refresh();
    }
}
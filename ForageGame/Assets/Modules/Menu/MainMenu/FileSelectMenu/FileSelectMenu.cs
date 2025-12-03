using UnityEngine;

public class FileSelectMenu : MonoBehaviour, IMenu
{
    // ------------ MENU PANELS ------------
    [Header("Menu Panels")]
    [SerializeField] private MainMenu mainMenu;

    // ------------ OTHER COMPONENTS ------------
    [Header("Misc")]
    [SerializeField] private FileSlotUI[] fileSlots;

    // ------------ IMenu ------------
    public void OpenMenu()
    {
        gameObject.SetActive(true);
        foreach (FileSlotUI fileSlot in fileSlots)
            fileSlot.Refresh();
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    // ------------ BUTTONS ------------

    public void Back_Button()
    {
        CloseMenu();
        mainMenu.OpenMenu();
    }

    public void Continue_Button(int slot)
    {
        SaveSystem.LoadSaveFile(slot);
    }

    public void NewGame_Button(int slot)
    {
        SaveSystem.NewSaveFile(slot);
    }

    public void Delete_Button(int slot)
    {
        SaveSystem.DeleteSaveFile(slot);
    }

    // ------------ OTHER FUNCTIONS ------------
}

using Unity.VisualScripting;
using UnityEngine;

public class CreditsMenu : MonoBehaviour, IMenu
{
    // ------------ MENU PANELS ------------
    [Header("Menu Panels")]
    [SerializeField] private MainMenu mainMenu;

    // ------------ IMenu ------------
    public void OpenMenu()
    {
        gameObject.SetActive(true);
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

    // ------------ OTHER FUNCTIONS ------------
}

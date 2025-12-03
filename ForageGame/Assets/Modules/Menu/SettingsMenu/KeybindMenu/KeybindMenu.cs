using UnityEngine;
using UnityEngine.InputSystem;

public class KeybindMenu : MonoBehaviour, IMenu
{
    // ------------ MENU PANELS ------------
    [Header("Menu Panels")]
    [SerializeField] private AudioMenu audioMenu;
    [SerializeField] private GraphicsMenu graphicsMenu;

    // ------------ OTHER COMPONENTS ------------
    [SerializeField] private KeyRebindUI[] keyRebindUIs;

    // ------------ IMenu ------------
    public void OpenMenu()
    {
        gameObject.SetActive(true);
        foreach (KeyRebindUI keyRebindUI in keyRebindUIs)
            keyRebindUI.Refresh();
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    // ------------ BUTTONS ------------

    public void Audio_Button()
    {
        CloseMenu();
        audioMenu.OpenMenu();
    }

    public void Graphics_Button()
    {
        CloseMenu();
        graphicsMenu.OpenMenu();
    }

    public void ResetToDefault(InputActionAsset actions)
    {
        foreach (var map in actions.actionMaps)
            map.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey("rebinds");
    }

    // ------------ OTHER FUNCTIONS ------------
}

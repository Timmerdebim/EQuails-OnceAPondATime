using UnityEngine;

public class GraphicsMenu : MonoBehaviour, IMenu
{
    // ------------ MENU PANELS ------------
    [Header("Menu Panels")]
    [SerializeField] private AudioMenu audioMenu;
    [SerializeField] private KeybindMenu keybindMenu;

    // ------------ OTHER COMPONENTS ------------

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

    public void Audio_Button()
    {
        CloseMenu();
        audioMenu.OpenMenu();
    }

    public void Keybind_Button()
    {
        CloseMenu();
        keybindMenu.OpenMenu();
    }

    public void SetFullscreen(bool full)
    {
        Screen.fullScreen = full;
        SettingsManager.Fullscreen = full;
        SettingsManager.Save();
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        SettingsManager.QualityIndex = index;
        SettingsManager.Save();
    }

    public void SetVSync(bool on)
    {
        QualitySettings.vSyncCount = on ? 1 : 0;
        SettingsManager.VSync = on;
        SettingsManager.Save();
    }

    // ------------ OTHER FUNCTIONS ------------
}

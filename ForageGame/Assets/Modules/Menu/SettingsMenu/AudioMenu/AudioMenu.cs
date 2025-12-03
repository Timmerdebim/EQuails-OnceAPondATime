using UnityEngine;
using UnityEngine.Audio;

public class AudioMenu : MonoBehaviour, IMenu
{
    // ------------ MENU PANELS ------------
    [Header("Menu Panels")]
    [SerializeField] private GraphicsMenu graphicsMenu;
    [SerializeField] private KeybindMenu keybindMenu;

    // ------------ OTHER COMPONENTS ------------

    [SerializeField] private AudioMixer mixer;

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

    public void Graphics_Button()
    {
        CloseMenu();
        graphicsMenu.OpenMenu();
    }

    public void Keybind_Button()
    {
        CloseMenu();
        keybindMenu.OpenMenu();
    }

    public void SetMasterVolume(float value)
    {
        mixer.SetFloat("MasterVol", Mathf.Log10(value) * 20);
        SettingsManager.MasterVolume = Mathf.Log10(value) * 20;
        SettingsManager.Save();
    }

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(value) * 20);
        SettingsManager.MusicVolume = Mathf.Log10(value) * 20;
        SettingsManager.Save();
    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFXVol", Mathf.Log10(value) * 20);
        SettingsManager.SFXVolume = Mathf.Log10(value) * 20;
        SettingsManager.Save();
    }

    // ------------ OTHER FUNCTIONS ------------
}

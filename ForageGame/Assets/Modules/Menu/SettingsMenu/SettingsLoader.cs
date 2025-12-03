using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class SettingsLoader : MonoBehaviour
{
    void Awake()
    {
        LoadGraphicsSettings();
        LoadAudioSettings();
        LoadRebinds();
    }

    // ------------ GRAPHICS ------------

    public void LoadGraphicsSettings()
    {
        Screen.fullScreen = SettingsManager.Fullscreen;
        QualitySettings.SetQualityLevel(SettingsManager.QualityIndex);
        QualitySettings.vSyncCount = SettingsManager.VSync ? 1 : 0;
    }

    // ------------ AUDIO ------------

    [SerializeField] private AudioMixer audioMixer;

    public void LoadAudioSettings()
    {
        audioMixer.SetFloat("MasterVol", SettingsManager.MasterVolume);
        audioMixer.SetFloat("MusicVol", SettingsManager.MusicVolume);
        audioMixer.SetFloat("SFXVol", SettingsManager.SFXVolume);
    }

    // ------------ KEYBINDS ------------

    [SerializeField] private InputActionAsset actions;

    public void LoadRebinds()
    {
        if (!PlayerPrefs.HasKey("rebinds"))
            return;

        string json = PlayerPrefs.GetString("rebinds");
        actions.LoadBindingOverridesFromJson(json);
    }
}

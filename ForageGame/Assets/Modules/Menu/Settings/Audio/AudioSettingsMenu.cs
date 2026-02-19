using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioSettingsMenu : Menu
{
    [Header("UI References")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Text masterVolumeText;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Text musicVolumeText;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Text sfxVolumeText;

    public override void EnteringMenu()
    {
        base.EnteringMenu();
        RefreshVisuals();
    }

    // ------------ Buttons ------------

    public void OnMasterVolumeChanged()
    {
        float value = masterVolumeSlider.value;
        masterVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        AudioSettingsManager.Instance?.SetMasterVolume(value);
    }

    public void OnMusicVolumeChanged()
    {
        float value = musicVolumeSlider.value;
        musicVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        AudioSettingsManager.Instance?.SetMusicVolume(value);
    }

    public void OnSfxVolumeChanged()
    {
        float value = sfxVolumeSlider.value;
        sfxVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        AudioSettingsManager.Instance?.SetSfxVolume(value);
    }

    // ------------ Functions ------------

    private void RefreshVisuals()
    {
        AudioSettings settings = AudioSettingsManager.Instance._settings;
        masterVolumeSlider.value = settings.masterVolume;
        masterVolumeText.text = Mathf.RoundToInt(settings.masterVolume * 100) + "%";
        musicVolumeSlider.value = settings.musicVolume;
        musicVolumeText.text = Mathf.RoundToInt(settings.musicVolume * 100) + "%";
        sfxVolumeSlider.value = settings.sfxVolume;
        sfxVolumeText.text = Mathf.RoundToInt(settings.sfxVolume * 100) + "%";
    }
}
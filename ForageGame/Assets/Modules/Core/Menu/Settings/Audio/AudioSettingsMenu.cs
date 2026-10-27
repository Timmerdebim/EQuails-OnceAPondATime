using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Project.Menus.Audio
{
    public class AudioSettingsMenu : Menu
    {
        [Header("UI References")]
        [SerializeField] private SliderUIElement _masterVolume;
        [SerializeField] private SliderUIElement _musicVolume;
        [SerializeField] private SliderUIElement _sfxVolume;
        [SerializeField] private SliderUIElement _ambienceVolume;

        public override void OnEnteringMenu()
        {
            _masterVolume.SetSliderValue(AudioSettingsManager.Instance.MasterVolume);
            _musicVolume.SetSliderValue(AudioSettingsManager.Instance.MusicVolume);
            _sfxVolume.SetSliderValue(AudioSettingsManager.Instance.SfxVolume);
            _ambienceVolume.SetSliderValue(AudioSettingsManager.Instance.AmbienceVolume);
        }

        // ------------ Buttons ------------

        public void OnMasterVolumeChanged()
        {
            AudioSettingsManager.Instance.MasterVolume = Mathf.RoundToInt(_masterVolume.value);
        }

        public void OnMusicVolumeChanged()
        {
            AudioSettingsManager.Instance.MusicVolume = Mathf.RoundToInt(_musicVolume.value);
        }

        public void OnSfxVolumeChanged()
        {
            AudioSettingsManager.Instance.SfxVolume = Mathf.RoundToInt(_sfxVolume.value);
        }

        public void OnAmbienceVolumeChanged()
        {
            AudioSettingsManager.Instance.AmbienceVolume = Mathf.RoundToInt(_ambienceVolume.value);
        }

        // ------------ Functions ------------
    }
}
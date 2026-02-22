using System.IO;
using UnityEngine;
using UnityEngine.Audio;

namespace Project.Menus.Audio
{
    public class AudioSettingsManager : MonoBehaviour
    {
        public static AudioSettingsManager Instance { get; private set; }
        [SerializeField] private string settingsPath = "Assets/Settings";
        [SerializeField] private AudioMixer audioMixer;
        public AudioSettings _settings { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            //DontDestroyOnLoad(gameObject);

            LoadSettings();
            ApplyAllSettings();
        }

        // ------------ Setting Functions ------------

        public void SetMasterVolume(float value)
        {
            _settings.masterVolume = value;
            audioMixer.SetFloat("Master", value);
        }

        public void SetMusicVolume(float value)
        {
            _settings.musicVolume = value;
            audioMixer.SetFloat("Music", value);
        }

        public void SetSfxVolume(float value)
        {
            _settings.sfxVolume = value;
            audioMixer.SetFloat("SFX", value);
        }

        public void ApplyAllSettings()
        {
            SetMasterVolume(_settings.masterVolume);
            SetMusicVolume(_settings.musicVolume);
            SetSfxVolume(_settings.sfxVolume);
        }

        // ------------ Save & Load ------------

        public void LoadSettings()
        {
            string audioPath = Path.Combine(settingsPath, "audio.json");

            if (File.Exists(audioPath))
            {
                string json = File.ReadAllText(audioPath);
                _settings = JsonUtility.FromJson<AudioSettings>(json);
            }
            else
                _settings = new AudioSettings();
        }

        public void SaveSettings()
        {
            if (!Directory.Exists(settingsPath))
                Directory.CreateDirectory(settingsPath);

            string settingsJson = JsonUtility.ToJson(_settings, true);

            File.WriteAllText(Path.Combine(settingsPath, "audio.json"), settingsJson);
        }
    }
}
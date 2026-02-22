using System.IO;
using UnityEngine;
using UnityEngine.Audio;

namespace Project.Menus.Audio
{
    public class AudioSettingsManager : MonoBehaviour
    {
        public static AudioSettingsManager Instance { get; private set; }
        [SerializeField] private string settingsPath = "Assets/SaveData";
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

            LoadSettings();
        }

        // ------------ Settings ------------

        public float MasterVolume
        {
            get => _settings.masterVolume;
            set
            {
                _settings.masterVolume = value;
                audioMixer.SetFloat("Master", value);
            }
        }

        public float MusicVolume
        {
            get => _settings.musicVolume;
            set
            {
                _settings.musicVolume = value;
                audioMixer.SetFloat("Music", value);
                SaveSettings();
            }
        }

        public float SfxVolume
        {
            get => _settings.sfxVolume;
            set
            {
                _settings.sfxVolume = value;
                audioMixer.SetFloat("SFX", value);
                SaveSettings();
            }
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
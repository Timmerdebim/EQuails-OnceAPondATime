using System.IO;
using UnityEngine;
using UnityEngine.Audio;

namespace Project.Menus.Audio
{
    public class AudioSettingsManager : MonoBehaviour
    {
        public static AudioSettingsManager Instance { get; private set; }
        [SerializeField] private string settingsPath = "Assets/SaveData";
        public AudioSettings _settings { get; private set; }

        [Header("FMOD Studio Stuff")]

        [SerializeField] private string masterBusPath = "bus:/";
        private FMOD.Studio.Bus _masterBus;
        [SerializeField] private string musicBusPath;
        private FMOD.Studio.Bus _musicBus;
        [SerializeField] private string sfxBusPath;
        private FMOD.Studio.Bus _sfxBus;
        [SerializeField] private string ambienceBusPath;
        private FMOD.Studio.Bus _ambienceBus;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _masterBus = FMODUnity.RuntimeManager.GetBus(masterBusPath);
            _musicBus = FMODUnity.RuntimeManager.GetBus(musicBusPath);
            _ambienceBus = FMODUnity.RuntimeManager.GetBus(ambienceBusPath);
            _sfxBus = FMODUnity.RuntimeManager.GetBus(sfxBusPath);

            LoadSettings();
        }

        // ------------ Settings ------------

        public float MasterVolume
        {
            get => _settings.masterVolume;
            set
            {
                _settings.masterVolume = value;
                _masterBus.setVolume(value);
                SaveSettings();
            }
        }

        public float MusicVolume
        {
            get => _settings.musicVolume;
            set
            {
                _settings.musicVolume = value;
                _musicBus.setVolume(value);
                SaveSettings();
            }
        }

        public float SfxVolume
        {
            get => _settings.sfxVolume;
            set
            {
                _settings.sfxVolume = value;
                _sfxBus.setVolume(value);
                SaveSettings();
            }
        }

        public float AmbienceVolume
        {
            get => _settings.ambienceVolume;
            set
            {
                _settings.ambienceVolume = value;
                _ambienceBus.setVolume(value);
                SaveSettings();
            }
        }

        private void ApplySettings()
        {
            MasterVolume = MasterVolume;
            MusicVolume = MusicVolume;
            SfxVolume = SfxVolume;
            AmbienceVolume = AmbienceVolume;
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

            ApplySettings();
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
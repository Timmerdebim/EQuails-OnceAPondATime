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

        [SerializeField] private string masterBusPath = "bus:/";
        private FMOD.Studio.Bus _masterBus;

        [SerializeField] private string musicBusPath;
        private FMOD.Studio.Bus _musicBus;

        [SerializeField] private string sfxBusPath;
        private FMOD.Studio.Bus _sfxBus;
        [SerializeField] private string ambienceBusPath;
        private FMOD.Studio.Bus _ambienceBus;

        public AudioSettings _settings { get; private set; }

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
                //audioMixer.SetFloat("Master", value);
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
                //audioMixer.SetFloat("Music", value);
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
                //audioMixer.SetFloat("SFX", value);
                _sfxBus.setVolume(value);
                SaveSettings();
            }
        }

        //TODO: fill in and fix
        public float AmbienceVolume
        {
            get => _settings.sfxVolume;
            set
            {
                _settings.sfxVolume = value;
                //audioMixer.SetFloat("SFX", value);
                _ambienceBus.setVolume(value);
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
using UnityEngine;
using System.IO;

namespace Project.Menus.Graphics
{
    public class GraphicsSettingsManager : MonoBehaviour
    {
        public static GraphicsSettingsManager Instance { get; private set; }
        [SerializeField] private string settingsPath = "Assets/Settings";
        public Resolution[] _resolutions { get; private set; }
        public GraphicsSettings _settings { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            //DontDestroyOnLoad(gameObject);

            _resolutions = Screen.resolutions;

            LoadSettings();
            ApplyAllSettings();
        }

        // ------------ Setting Functions ------------
        public void SetResolution(int index)
        {
            if (-1 < index && index < _resolutions.Length)
            {
                _settings.resolutionIndex = index;
                Screen.SetResolution(_resolutions[index].width, _resolutions[index].height, Screen.fullScreen);
            }
        }

        public void SetQuality(int index)
        {
            _settings.qualityLevel = index;
            QualitySettings.SetQualityLevel(index);
        }

        public void SetVsync(bool isEnabled)
        {
            _settings.vsyncEnabled = isEnabled;
            QualitySettings.vSyncCount = isEnabled ? 1 : 0;
        }

        public void SetFramerate(int value)
        {
            _settings.targetFramerate = value;
            Application.targetFrameRate = value;
        }

        public void ApplyAllSettings()
        {
            SetResolution(_settings.resolutionIndex);
            SetQuality(_settings.qualityLevel);
            SetVsync(_settings.vsyncEnabled);
            SetFramerate(_settings.targetFramerate);
        }

        // ------------ Save & Load ------------

        public void LoadSettings()
        {
            string graphicsPath = Path.Combine(settingsPath, "graphics.json");

            if (File.Exists(graphicsPath))
            {
                string json = File.ReadAllText(graphicsPath);
                _settings = JsonUtility.FromJson<GraphicsSettings>(json);
            }
            else
                _settings = new GraphicsSettings();
        }

        public void SaveSettings()
        {
            if (!Directory.Exists(settingsPath))
                Directory.CreateDirectory(settingsPath);

            string settingsJson = JsonUtility.ToJson(_settings, true);

            File.WriteAllText(Path.Combine(settingsPath, "graphics.json"), settingsJson);
        }
    }
}
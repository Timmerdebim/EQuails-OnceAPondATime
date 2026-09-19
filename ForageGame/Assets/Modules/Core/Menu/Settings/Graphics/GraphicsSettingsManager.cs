using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Project.Menus.Graphics
{
    public class GraphicsSettingsManager : MonoBehaviour
    {
        [SerializeField] private RenderPipelineAsset[] _urpAssets; // low to high

        public static GraphicsSettingsManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Load & Apply All Settings
            FullScreen = FullScreen;
            Resolution = Resolution;
            Framerate = Framerate;
            TextureQuality = TextureQuality;
            LightingQuality = LightingQuality;
            TerrainQuality = TerrainQuality;
            VSync = VSync;
            AnisotropicTextures = AnisotropicTextures;
        }

        // ------------ Settings ------------

        public int FullScreen
        {
            // 0 = off, 1 = on.
            get => PlayerPrefs.GetInt("FullScreen", 1);
            set
            {
                Screen.fullScreen = (value == 1);
                PlayerPrefs.SetInt("FullScreen", value);
                PlayerPrefs.Save();
            }
        }

        public int Resolution
        {
            get => PlayerPrefs.GetInt("Resolution",
                // Default is the actual screen resolution (if possible), else its 1080p.
                GetDefaultResolutionIndex()
                );
            set
            {
                if (-1 < value && value < Screen.resolutions.Length)
                    Screen.SetResolution(Screen.resolutions[value].width, Screen.resolutions[value].height, Screen.fullScreen);

                PlayerPrefs.SetInt("Resolution", value);
                PlayerPrefs.Save();
            }
        }

        public int Framerate
        {
            get => PlayerPrefs.GetInt("Framerate", 60);
            set
            {
                Application.targetFrameRate = value;
                PlayerPrefs.SetInt("Framerate", value);
                PlayerPrefs.Save();
            }
        }

        public int TextureQuality
        {
            // 0 = full resolution, 1 = half, 2 = quarter, 3 = eighths. [FLIP THE LIST]
            // 0 = Terrible, 1 = Low, 2 = Medium, 3 = High.
            get => PlayerPrefs.GetInt("TextureQuality", 3);
            set
            {
                QualitySettings.globalTextureMipmapLimit = 3 - value;
                PlayerPrefs.SetInt("TextureQuality", value);
                PlayerPrefs.Save();
            }
        }

        public int LightingQuality
        {

            // 0 = Terrible, 1 = Low, 2 = Medium, 3 = High, 4 = Ridiculous.
            get => PlayerPrefs.GetInt("TextureQuality", 3);
            set
            {
                if (_urpAssets[value] != null)
                    QualitySettings.renderPipeline = _urpAssets[value];
                PlayerPrefs.SetInt("TextureQuality", value);
                PlayerPrefs.Save();
            }
        }

        public int TerrainQuality
        {
            // 0 = Terrible, 1 = Low, 2 = Medium, 3 = High.
            get => PlayerPrefs.GetInt("TerrainQuality", 3);
            set
            {
                if (value == 0) QualitySettings.terrainPixelError = 20;
                else if (value == 1) QualitySettings.terrainPixelError = 10;
                else if (value == 2) QualitySettings.terrainPixelError = 5;
                else if (value == 3) QualitySettings.terrainPixelError = 1;
                PlayerPrefs.SetInt("TerrainQuality", value);
                PlayerPrefs.Save();
            }
        }

        public int VSync
        {
            // 0 = off, 1 = on.
            get => PlayerPrefs.GetInt("VSync", 0);
            set
            {
                QualitySettings.vSyncCount = value;
                PlayerPrefs.SetInt("VSync", value);
                PlayerPrefs.Save();
            }
        }

        public int AnisotropicTextures
        {
            // 0 = Off, 1 = On.
            get => PlayerPrefs.GetInt("AnisotropicTextures", 1);
            set
            {
                if (value == 0) QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                else QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                PlayerPrefs.SetInt("AnisotropicTextures", value);
                PlayerPrefs.Save();
            }
        }

        // ------- HELPERS ------

        private static int GetDefaultResolutionIndex()
        {
            Resolution current = Screen.currentResolution;

            // Prefer the current screen resolution.
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                if (Screen.resolutions[i].width == current.width &&
                    Screen.resolutions[i].height == current.height)
                {
                    return i;
                }
            }

            // Otherwise fall back to 1920x1080.
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                if (Screen.resolutions[i].width == 1920 &&
                    Screen.resolutions[i].height == 1080)
                {
                    return i;
                }
            }

            // Last resort: first available resolution.
            return Screen.resolutions.Length > 0 ? 0 : -1;
        }
    }
}
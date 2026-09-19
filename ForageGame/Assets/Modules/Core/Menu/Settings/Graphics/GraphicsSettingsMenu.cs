using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using System;

namespace Project.Menus.Graphics
{
    public class GraphicsSettingsMenu : Menu
    {
        [Header("UI References")]
        [SerializeField] private SelectionUIElement _fullscreen;
        [SerializeField] private SelectionUIElement _resolution;
        [SerializeField] private Slider _framerate;
        [SerializeField] private SelectionUIElement _textureQuality;
        [SerializeField] private SelectionUIElement _lightingQuality;
        [SerializeField] private SelectionUIElement _terrainQuality;
        [SerializeField] private SelectionUIElement _vSync;
        [SerializeField] private SelectionUIElement _anisotropicTextures;

        public override void OnEnteringMenu()
        {
            // Resolution
            List<string> resolutionOptions = new();
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                string resolutionOption = $"{Screen.resolutions[i].width}x{Screen.resolutions[i].height}";
                resolutionOptions.Add(resolutionOption);
            }
            _resolution.SetOptions(resolutionOptions.ToArray());

            _fullscreen.SetOptions(new string[] { "Off", "On" });
            _textureQuality.SetOptions(new string[] { "Terrible", "Low", "Medium", "High" });
            _lightingQuality.SetOptions(new string[] { "Unplayable", "Low", "Medium", "High", "Ridiculous" });
            _terrainQuality.SetOptions(new string[] { "Too Low", "Low", "Medium", "High" });
            _vSync.SetOptions(new string[] { "Off", "On" });
            _anisotropicTextures.SetOptions(new string[] { "Off", "On" });

            RefreshVisuals();
        }

        // ------------ Buttons ------------

        public void OnFullscreenChanged() => GraphicsSettingsManager.Instance.FullScreen = _fullscreen._currentOption;
        public void OnResolutionChanged() => GraphicsSettingsManager.Instance.Resolution = _resolution._currentOption;
        public void OnFramerateChanged() => GraphicsSettingsManager.Instance.Framerate = Mathf.RoundToInt(_framerate.value);
        public void OnTextureQualityChanged() => GraphicsSettingsManager.Instance.TextureQuality = _textureQuality._currentOption;
        public void OnLightingQualityChanged() => GraphicsSettingsManager.Instance.LightingQuality = _lightingQuality._currentOption;
        public void OnTerrainQualityChanged() => GraphicsSettingsManager.Instance.TerrainQuality = _terrainQuality._currentOption;
        public void OnVSyncChanged() => GraphicsSettingsManager.Instance.VSync = _vSync._currentOption;
        public void OnAnisotropicTexturesChanged() => GraphicsSettingsManager.Instance.AnisotropicTextures = _anisotropicTextures._currentOption;

        // ------------ Functions ------------

        public void RefreshVisuals()
        {
            _fullscreen.SetCurrentOption(GraphicsSettingsManager.Instance.FullScreen);
            _resolution.SetCurrentOption(GraphicsSettingsManager.Instance.Resolution);
            _framerate.value = GraphicsSettingsManager.Instance.Framerate;
            _textureQuality.SetCurrentOption(GraphicsSettingsManager.Instance.TextureQuality);
            _lightingQuality.SetCurrentOption(GraphicsSettingsManager.Instance.LightingQuality);
            _terrainQuality.SetCurrentOption(GraphicsSettingsManager.Instance.TerrainQuality);
            _vSync.SetCurrentOption(GraphicsSettingsManager.Instance.VSync);
            _anisotropicTextures.SetCurrentOption(GraphicsSettingsManager.Instance.AnisotropicTextures);
        }
    }
}
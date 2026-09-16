using UnityEngine;
using System.Collections;
using TDK.PlayerSystem;
using FMODUnity;
using AudioIntegration;

namespace Weather
{
    public class ThunderstormWindSettings : WeatherBehaviour
    {
        [SerializeField] private Material homeTreeMaterial;
        [SerializeField] private Material homeFoliageMaterial;

        [Header("Thunderstorm Wind Settings")]

        [SerializeField] private WindState treeWindSettings;
        [SerializeField] private WindState foliageWindSettings;

        [System.Serializable]
        private struct WindState
        {
            public float blast, intensity, ripples, speed, wavelength, turbulence, yaw;
        }

        public override void SetBlend(float value)
        {
            //this actually does nothing, since this behavior is just an on-off type deal.
        }

        private WindState _originalTree;
        private WindState _originalFoliage;

        private void OnEnable()
        {
            _originalTree    = ReadState(homeTreeMaterial);
            _originalFoliage = ReadState(homeFoliageMaterial);

            ApplyState(homeTreeMaterial,    treeWindSettings);
            ApplyState(homeFoliageMaterial, foliageWindSettings);
        }

        //resets the state of the material, let's hope this actually works
        private void OnDisable()
        {
            RestoreState(homeTreeMaterial,    _originalTree);
            RestoreState(homeFoliageMaterial, _originalFoliage);
        }

        private WindState ReadState(Material mat) => new WindState
        {
            blast      = mat.GetFloat("_Wind_Blast"),
            intensity  = mat.GetFloat("_Wind_Intensity"),
            ripples    = mat.GetFloat("_Wind_Ripples"),
            speed      = mat.GetFloat("_Wind_Speed"),
            wavelength = mat.GetFloat("_Wind_Wavelength"),
            turbulence = mat.GetFloat("_Wind_Turbulence"),
            yaw        = mat.GetFloat("_Wind_Yaw")
        };

        private void ApplyState(Material mat, WindState windState)
        {
            mat.SetFloat("_Wind_Blast",      windState.blast);
            mat.SetFloat("_Wind_Intensity",  windState.intensity);
            mat.SetFloat("_Wind_Ripples",    windState.ripples);
            mat.SetFloat("_Wind_Speed",      windState.speed);
            mat.SetFloat("_Wind_Wavelength", windState.wavelength);
            mat.SetFloat("_Wind_Turbulence", windState.turbulence);
            mat.SetFloat("_Wind_Yaw",        windState.yaw);
        }

        private void RestoreState(Material mat, WindState s) =>
            ApplyState(mat, s);
    }
}
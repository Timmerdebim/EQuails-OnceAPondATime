using DG.Tweening;
using DG.Tweening.Core;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Lantern : MonoBehaviour
{
    [SerializeField] Light light;

    [SerializeField] private Color mutedColor = Color.red;
    [SerializeField] private Color BrightColor = Color.yellow;

    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 5f;

    [SerializeField] [Range(0f, 1f)] private float lanternStrength = 0.5f;

    [SerializeField] [Range(0f, 10f)] private float flickerSpeed = 1f;

    FBM1D fbm;
    [SerializeField] FBM1D.FBMSettings FBMSettings = new FBM1D.FBMSettings(FBM1D.NoiseFunctionType.Sin, 4, 1.97f, 0.43f);

    private void Start()
    {
        fbm = new FBM1D(FBMSettings);
    }

    private void Update()
    {
        lanternStrength = fbm.Eval01(Time.time * flickerSpeed);
        SetLanternVisuals();
    }

    [ContextMenu("Set Lantern Visuals")]
    private void SetLanternVisuals()
    {
        light.color = Color.Lerp(mutedColor, BrightColor, lanternStrength);
        light.intensity = Mathf.Lerp(minIntensity, maxIntensity, lanternStrength);
    }

}

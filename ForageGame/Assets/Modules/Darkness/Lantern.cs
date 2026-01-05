using DG.Tweening;
using DG.Tweening.Core;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Lantern : MonoBehaviour
{
    [SerializeField] Light light;

    private float initialIntensity;
    private float initialRange;

    [SerializeField] private Color mutedColor = Color.red;
    [SerializeField] private Color BrightColor = Color.yellow;

    [SerializeField] private float flickerAvgPeriod = 0.1f;

    [SerializeField] private float intensityFlickerVariance = 1f;

    [SerializeField] [Range(0f, 1f)] private float lanternStrength = 0.5f;

    private void Start()
    {
        initialIntensity = light.intensity;
        initialRange = light.range;

        Animations();
    }

    private void Update()
    {
        SetLanternVisuals();
    }

    [ContextMenu("Set Lantern Visuals")]
    private void SetLanternVisuals()
    {
        light.color = Color.Lerp(mutedColor, BrightColor, lanternStrength);
        light.intensity = Mathf.Lerp(initialIntensity - intensityFlickerVariance, initialIntensity + intensityFlickerVariance, lanternStrength);
    }

    private async void Animations()
    {
        while (true)
        {
            float currentStrength = lanternStrength;
            float targetStrength = -1f;
            while (targetStrength < 0f || targetStrength > 1f)
            {
                targetStrength = NormalFromUniform(Random.value) - 0.5f + currentStrength; // shift the mean to currentStrength
            }

            float delay = flickerAvgPeriod * (1 + NormalFromUniform(Random.value)); // mean at flickerAvgPeriod

            lanternStrength = targetStrength;

            await Task.Delay(Mathf.RoundToInt(Mathf.Clamp(delay * 1000, 0, Mathf.Infinity)));


            //Tween flicker = DOTween.To(() => lanternStrength, x => lanternStrength = x, targetStrength, delay)
            //    .SetEase(Ease.InOutSine);

            //await flicker.AsyncWaitForCompletion();
        }

    }

    private float NormalFromUniform(float value)
    {
        return Mathf.Sqrt(2) * ErfInv(2 * value - 1);
    }

    private static float ErfInv(float x)
    {
        // domain check
        if (x <= -1f) return float.NegativeInfinity;
        if (x >= 1f) return float.PositiveInfinity;

        float a = 0.147f; // magic constant
        float ln = Mathf.Log(1f - x * x);
        float first = 2f / (Mathf.PI * a) + ln / 2f;
        float second = ln / a;

        return Mathf.Sign(x) * Mathf.Sqrt(
            Mathf.Sqrt(first * first - second) - first
        );
    }

    //private async Task Animations()
    //{
    //    // Start a new IntensityFlicker when the previous one ends, and the same for a ColorFlicker
    //    initialIntensity = light.intensity;
    //    initialRange = light.range;
    //    while (true)
    //    {
    //        Tween intensityFlicker = IntensityFlicker();
    //        Tween colorFlicker = ColorFlicker();

    //        await Task.WhenAll(
    //            intensityFlicker.AsyncWaitForCompletion(),
    //            colorFlicker.AsyncWaitForCompletion()
    //        );
    //    }
    //}

    //private Tween IntensityFlicker()
    //{
    //    float delay =  flickerAvgPeriod * (0.5f + Random.value - 0.5f);
    //    float targetIntensity = initialIntensity + Random.Range(-flicketAmplitude, flicketAmplitude);

    //    Tween intensityFlicker = light.DOIntensity(targetIntensity, delay).SetEase(Ease.InOutBounce);

    //    return intensityFlicker;
    //}

    //private Tween ColorFlicker()
    //{
    //    float delay = flickerAvgPeriod * (0.5f + Random.value - 0.5f);
    //    Color randomColor = Color.Lerp(blendColor1, blendColor2, Random.value);
    //    Color targetColor = Color.Lerp(light.color, randomColor, Random.value);

    //    Tween colorFlicker = light.DOColor(targetColor, delay).SetEase(Ease.InOutBounce);

    //    return colorFlicker;
    //}
}

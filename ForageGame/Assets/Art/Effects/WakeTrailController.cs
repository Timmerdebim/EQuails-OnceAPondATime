using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WakeTrailController : MonoBehaviour
{
    [SerializeField] TrailRenderer trail;
    [SerializeField] Gradient baseGradient;
    [SerializeField] float stopFadeDuration = 0.15f;

    Coroutine fadeRoutine;


    void Awake()
    {
        trail.emitting = false;
    }

    public void SetSwimming(bool swimming)
    {
        trail.emitting = swimming;

        if (!swimming)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeOutTrail());
        }
        else if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            trail.colorGradient = baseGradient; //restore base gradient
        }
    }

    IEnumerator FadeOutTrail()
    {
        float t = 0f;
        while (t < stopFadeDuration)
        {
            t += Time.deltaTime;
            float mult = 1f - (t / stopFadeDuration);
            trail.colorGradient = ScaleGradientAlpha(baseGradient, mult);
            yield return null;
        }
        trail.Clear();
        trail.colorGradient = baseGradient; //restore base gradient
    }

    //applies alpha multiplier to a gradient
    Gradient ScaleGradientAlpha(Gradient src, float mult)
    {
        var g = new Gradient();
        var colorKeys = src.colorKeys;
        var alphaKeys = src.alphaKeys;
        for (int i = 0; i < alphaKeys.Length; i++)
            alphaKeys[i].alpha *= mult;
        g.SetKeys(colorKeys, alphaKeys);
        return g;
    }
}

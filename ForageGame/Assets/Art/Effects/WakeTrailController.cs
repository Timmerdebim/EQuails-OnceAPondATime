using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WakeTrailController : MonoBehaviour
{
    [SerializeField] TrailRenderer trail;
    [SerializeField] Gradient baseGradient;
    [SerializeField] float stopFadeDuration = 0.15f;

    [SerializeField] public Vector3 targetLocalPosition = Vector3.zero;
    [SerializeField] public float positionSmoothSpeed = .1f;
    public bool IsEmitting() => trail.emitting; //just a getter

    Coroutine fadeRoutine;


    void Awake()
    {
        trail.emitting = false;
    }

    void Update()
    {
        if(IsEmitting())
        {
            if (!Vector3.Equals(transform.localPosition, targetLocalPosition))
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetLocalPosition, positionSmoothSpeed);
            }
        }
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

    //copy of above, used for when the player stops moving (trailrenderer looks odd without it)
    public void FadeOutTail()
    {
        if(trail.emitting)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeOutTrail());
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

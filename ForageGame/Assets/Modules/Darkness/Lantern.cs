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

    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 5f;

    [SerializeField] [Range(0f, 1f)] private float lanternStrength = 0.5f;

    FBM1D fbm = new FBM1D(Mathf.Sin, 4, 1.97f, 0.43f);

    private void Start()
    {
        initialIntensity = light.intensity;
        initialRange = light.range;

        //Animations();
    }

    private void Update()
    {
        lanternStrength = fbm.Eval01(Time.time);
        SetLanternVisuals();
    }

    [ContextMenu("Set Lantern Visuals")]
    private void SetLanternVisuals()
    {
        light.color = Color.Lerp(mutedColor, BrightColor, lanternStrength);
        light.intensity = Mathf.Lerp(minIntensity, maxIntensity, lanternStrength);
    }

}

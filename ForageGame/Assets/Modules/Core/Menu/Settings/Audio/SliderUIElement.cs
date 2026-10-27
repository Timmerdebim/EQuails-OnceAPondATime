using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class SliderUIElement : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private SliderUIText _text;

    public float value => _slider.value;

    [Header("Sfx")]
    [SerializeField] private EventReference scribbleEvent;
    EventInstance _inst;
    float _lastTime;
    [SerializeField] float IdleTimeout = 0.12f;


    //slider itself was changed
    public void OnSliderValueChanged()
    {
        _text.OnValueChanged();
        float now = Time.unscaledTime;
        float dt = Mathf.Max(now - _lastTime, 0.001f);
        _lastTime = now;

        if (!_inst.isValid())
        {
            _inst = RuntimeManager.CreateInstance(scribbleEvent);
            _inst.start();
        }
    }

    //changed from outside source, i.e. menu populating. Play no SFX
    public void SetSliderValue(float v)
    {
        _slider.SetValueWithoutNotify(v);
        _text.OnValueChanged();
    }

    void Update()
    {
        if (_inst.isValid() && Time.unscaledTime - _lastTime > IdleTimeout)
        {
            StopAndRelease();
        }
    }

    void OnDisable() => StopAndRelease();
    private void StopAndRelease()
    {
        if (_inst.isValid())
        {
            _inst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _inst.release();
        }
    }

}

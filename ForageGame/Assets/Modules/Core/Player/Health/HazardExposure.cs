using System;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using AudioIntegration;

/// <summary>
/// Received for HazardZone
/// Is a simple generic exposure timer, shared across damaging areas
/// Mostly to have a bit of grace so the player can respond to being inside the hazard area before taking damage
/// </summary>
public class HazardExposure : MonoBehaviour
{
    [SerializeField] int damage = 8;
    [SerializeField] float buildUpTime = 0.5f;
    [SerializeField] float decayDelay = 0.5f;
    [SerializeField] float decayTime = 2f;

    float exposure, timeOutside;
    int overlaps;
    [SerializeField] private IHitHandler health;

    void Awake() => health = GetComponent<IHitHandler>();

    public void Enter() => overlaps++;
    public void Exit()  => overlaps = Mathf.Max(0, overlaps - 1);

    void Update()
    {
        if (overlaps > 0)
        {
            timeOutside = 0;
            exposure = Mathf.Min(1f, exposure + Time.deltaTime / buildUpTime);
            if (exposure >= 1f) health.Hit(damage); //Iframes throttle this anyway, and is how old system worked
        }
        else
        {
            timeOutside += Time.deltaTime;
            if (timeOutside > decayDelay)
                exposure = Mathf.Max(0f, exposure - Time.deltaTime / decayTime);
        }
    }
}
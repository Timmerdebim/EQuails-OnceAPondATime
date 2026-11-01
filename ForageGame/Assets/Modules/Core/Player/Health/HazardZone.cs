using System;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using AudioIntegration;

/// <summary>
/// Simple variation of a hitbox for continuous damage areas
/// Doesn't do damage by itself, player's HazardExposure is responsible
/// </summary>
public class HazardZone : MonoBehaviour, IManagedAudioSource
{
    public LayerMask targetLayers;
    [SerializeField] private EventReference onTriggerStayEvent;
    private EventInstance onTriggerStay;

    void Awake()
    {
        onTriggerStay = RuntimeManager.CreateInstance(onTriggerStayEvent);
    }

    void OnTriggerEnter(Collider other)
    {
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;
        if (!other.TryGetComponent(out HazardExposure e)) return;
        e.Enter();
        
        onTriggerStay.start();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out HazardExposure e)) return;
        e.Exit();
        onTriggerStay.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void StopAndRelease()
    {
        onTriggerStay.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        onTriggerStay.release();
    }
}
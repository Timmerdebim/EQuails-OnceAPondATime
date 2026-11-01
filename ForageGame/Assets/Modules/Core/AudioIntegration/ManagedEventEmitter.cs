using UnityEngine;
using System.Collections.Generic;
using FMODUnity;
using System;

namespace AudioIntegration
{

    /// <summary>
    /// Simple component that makes simple (world) emitters managed, allowing them to be stopped on world unload
    /// 
    /// ~Lars
    /// </summary>
    [RequireComponent(typeof(StudioEventEmitter))]
    public class ManagedEventEmitter : MonoBehaviour, IManagedAudioSource
    {
        StudioEventEmitter studioEventEmitter;

        void OnEnable()  => AudioManager.Instance.Register(this);
        void OnDisable() => AudioManager.Instance.Unregister(this);

        void Awake()
        {
            studioEventEmitter = GetComponent<StudioEventEmitter>();
        }

        public void StopAndRelease()
        {
            studioEventEmitter.Stop();
        }
    }
}
using System;
using UnityEngine;

namespace TDK.EnemySystem
{
    [RequireComponent(typeof(Collider))]
    public class EnemyAudioSensor : EnemySensor
    {
        private int _colliderCounter = 0;

        void OnTriggerEnter(Collider other)
        {
            if ((_detectionMask.value & (1 << other.gameObject.layer)) != 0)
                UpdateColliderCounter(1);
        }

        void OnTriggerExit(Collider other)
        {
            if ((_detectionMask.value & (1 << other.gameObject.layer)) != 0)
                UpdateColliderCounter(-1);
        }

        private void UpdateColliderCounter(int value)
        {
            _colliderCounter = Math.Max(0, _colliderCounter + value);
            _sensorTriggered = _colliderCounter > 0;
        }
    }
}
using System;
using UnityEngine;

namespace TDK.EnemySystem
{
    public class EnemySensor : MonoBehaviour
    {
        [Header("Sensor Settings")]
        [SerializeField] protected LayerMask _detectionMask;
        [SerializeField] protected LayerMask _blockingMask;
        public bool _sensorTriggered { get; protected set; } = false;
    }
}
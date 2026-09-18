using TDK.PlayerSystem;
using UnityEngine;

namespace TDK.EnemySystem
{
    public class EnemyVisionSensor : EnemySensor
    {
        private Transform _player;
        [SerializeField] private float visionRadius = 10;

        void Start()
        {
            _player = Player.Instance.transform;
        }

        void Update()
        {
            if (Physics.SphereCast(
                transform.position, 0.2f,
                (_player.position - transform.position).normalized, out RaycastHit hit,
                visionRadius,
                _blockingMask | _detectionMask))
            {
                if ((_detectionMask.value & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    _sensorTriggered = true;
                    return;
                }
            }
            _sensorTriggered = false;
        }
    }
}
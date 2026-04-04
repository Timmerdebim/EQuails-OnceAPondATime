using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TDK.Physics3DSystem
{
    // We look at how fast we are traveling in a given direction and adjust our affectable velocity towards our target.
    [RequireComponent(typeof(Rigidbody))]
    public class VelocityDriver : MonoBehaviour
    {
        [Header("General Settings")]
        [SerializeField] private Vector3 _targetDirection = Vector3.forward;
        [SerializeField] private float _targetSpeed = 1;
        [SerializeField] private float _acceleration = 0;

        [Header("Affect Axes")]
        [SerializeField] private AffectedAxesMode _affectMode = AffectedAxesMode.All;
        [SerializeField] private Vector3 _affectNormal = Vector3.forward;
        [SerializeField] private bool _normalTracksTarget = true;
        public enum AffectedAxesMode { All, NormalPlane, NormalProjection }


        private Rigidbody _rigidbody;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Vector3 currentV = Vector3.Project(_rigidbody.linearVelocity, _targetVelocity);
        /// Vector3 deltaV = Vector3.MoveTowards(currentV, _targetVelocity, Time.fixedDeltaTime * _acceleration);
        /// _rigidbody.AddForce(deltaV, ForceMode.VelocityChange);
        /// </summary>
        void FixedUpdate()
        {
            _rigidbody.AddForce(
                Vector3.MoveTowards(
                    Vector3.zero,
                    GetRestrictedVelocity(_targetDirection * _targetSpeed - _rigidbody.linearVelocity),
                    Time.fixedDeltaTime * _acceleration),
                ForceMode.VelocityChange);
        }

        private Vector3 GetRestrictedVelocity(Vector3 velocity)
        {
            return _affectMode switch
            {
                AffectedAxesMode.All => velocity,
                AffectedAxesMode.NormalPlane => Vector3.ProjectOnPlane(velocity, _affectNormal),
                AffectedAxesMode.NormalProjection => Vector3.Project(velocity, _affectNormal),
                _ => Vector3.zero,
            };
        }

        #region API

        public void Reset()
        {
            _targetDirection = Vector3.forward;
            _targetSpeed = 1;
            _acceleration = 0;
            _affectMode = AffectedAxesMode.All;
            _affectNormal = Vector3.forward;
            _normalTracksTarget = true;
        }

        public void SetTargetVelocity(Vector3 targetVelocity)
        {
            SetTargetDirection(targetVelocity);
            SetTargetSpeed(targetVelocity.magnitude);
        }



        public void SetTargetDirection(Vector3 direction)
        {
            _targetDirection = direction.normalized;
            if (_normalTracksTarget) _affectNormal = _targetDirection;
        }
        public void SetTargetSpeed(float speed)
        {
            _targetSpeed = Mathf.Max(speed, 0);
        }
        public void SetAcceleration(float acceleration)
        {
            _acceleration = Mathf.Max(acceleration, 0);
        }
        public void SetAffectMode(AffectedAxesMode mode)
        {
            _affectMode = mode;
        }
        public void SetAffectNormal(Vector3 normal)
        {
            _affectNormal = normal.normalized;
        }
        public void SetNormalTracksTarget(bool normalTracksTarget)
        {
            _normalTracksTarget = normalTracksTarget;
            if (_normalTracksTarget) _affectNormal = _targetDirection;
        }

        #endregion
    }
}
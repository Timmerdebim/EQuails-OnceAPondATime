using UnityEngine;
using TDK.PlayerSystem;

namespace TDK.CameraSystem
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target Tracking (Orbital)")]
        public Transform _viewingTarget { get; private set; }
        [SerializeField] private float _translationalSpeed = 1;
        [SerializeField] private float _orbitalSpeed = 1;

        [Header("Player Tracking Mode")]
        [SerializeField] private Transform _playerCameraTarget;
        private bool _playerTrackingMode = true;
        [SerializeField] private float playerVelocityWeight = 1; // how much should the camera care about your velocity 
        [SerializeField] private float playerDirectionWeight = 1; // how much should the camera care about the firection you are facing

        void Start()
        {
            SetPlayerTarget();
            TeleportToTarget();
        }

        public void SetPlayerTarget() => SetTarget(_playerCameraTarget, true);

        public void SetTarget(Transform viewingTarget, bool playerTrackingMode = false)
        {
            _viewingTarget = viewingTarget;
            // get position & rotation from transform; 
            // target radius == target transform.localscale.x
            _playerTrackingMode = playerTrackingMode;
        }

        private Vector3 _targetPosition = Vector3.zero;
        private Quaternion _targetRotation = Quaternion.identity;
        private float _targetRadius = 0;
        private void RefreshTargetInfo()
        {
            if (_viewingTarget == null) return;
            _viewingTarget.GetPositionAndRotation(out _targetPosition, out _targetRotation);
            _targetRadius = _viewingTarget.localScale.x;
        }


        private Vector3 _targetTranslationPosition = Vector3.zero;
        private Vector3 _translationPosition = Vector3.zero;
        private Vector3 _orbitalPosition = Vector3.zero;
        private Quaternion _orbitalRotation = Quaternion.identity;

        public void TeleportToTarget()
        {
            RefreshTargetInfo();

            _orbitalRotation = _targetRotation;
            _orbitalPosition = _orbitalRotation * Vector3.forward * _targetRadius * (-1); // prior translational position: this is the anchor from which we "orbit";
            _translationPosition = _targetPosition;

            transform.SetPositionAndRotation(
                _orbitalPosition + _translationPosition,
                _orbitalRotation
                );
        }

        void LateUpdate()
        {
            RefreshTargetInfo();

            _orbitalRotation = Quaternion.Slerp(transform.rotation, _targetRotation, 1f - Mathf.Exp(-_orbitalSpeed * Time.unscaledDeltaTime));
            _orbitalPosition = _orbitalRotation * Vector3.forward * Mathf.Lerp(Vector3.Distance(transform.position, _translationPosition), _targetRadius, 1f - Mathf.Exp(-_orbitalSpeed * Time.unscaledDeltaTime)) * (-1); // prior translational position: this is the anchor from which we "orbit";

            _targetTranslationPosition = _targetPosition;
            if (_playerTrackingMode)
            {
                _targetTranslationPosition += playerVelocityWeight * Player.Instance.playerController._rigidbody.linearVelocity;
                _targetTranslationPosition += playerDirectionWeight * Player.Instance.playerController.ViewDirection;
            }
            _translationPosition = Vector3.Lerp(_translationPosition, _targetTranslationPosition, 1f - Mathf.Exp(-_translationalSpeed * Time.unscaledDeltaTime));

            transform.SetPositionAndRotation(
                _orbitalPosition + _translationPosition,
                _orbitalRotation
                );
        }
    }
}
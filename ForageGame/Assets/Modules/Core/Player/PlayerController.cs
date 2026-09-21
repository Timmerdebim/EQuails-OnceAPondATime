using System.Linq;
using NUnit.Framework.Constraints;
using TDK.Physics3DSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TDK.PlayerSystem
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(VelocityDriver))]
    [RequireComponent(typeof(PlayerAnimator))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private LayerMask physicsColliders;
        public Rigidbody _rigidbody { get; private set; }
        private PlayerAnimator _animator;
        [SerializeField] private PlayerVisuals _visuals;
        [SerializeField] private VelocityDriver _velocityDriver;

        public UnityEvent onJump;
        public UnityEvent onSprint;
        public UnityEvent onAttack;
        public UnityEvent<Vector3> onMove;
        public UnityEvent onLand;

        public UnityEvent<bool> onFootstep; //it's a mess; Animator events can only 'see' root functions and can't look deeper so yeah here we have another event

        public UnityEvent<bool> onSwimStroke;

        public UnityEvent<float> onWaterEnter;

        public UnityEvent onWaterLeave;

        //these are just for the water splash event (hitting water hard enough triggers it)
        private Vector3 _currentVelocity = Vector3.zero;
        private Vector3 _priorVelocity = Vector3.zero;




        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<PlayerAnimator>();
        }

        #region Move & View

        private Vector3 _inputVector = Vector3.zero;
        public Vector3 InputVector
        {
            get => _inputVector;
            set
            {
                _inputVector = Vector3.ClampMagnitude(value, 1);

                if (value.magnitude > 0.1f)
                {
                    ViewDirection = _inputVector;
                    _animator.IsMoving(true);
                    if (_vdTarget == VelocityDriverTarget.Input)
                        _velocityDriver.SetTargetDirection(InputVector);
                }
                else
                {
                    _animator.IsMoving(false);
                    if (_vdTarget == VelocityDriverTarget.Input)
                        _velocityDriver.SetTargetDirection(new(0, 0, 0));
                }
            }
        }
        private Vector3 _viewDirection = Vector3.right;
        public Vector3 ViewDirection
        {
            get => _viewDirection;
            set
            {
                _viewDirection = value.normalized;
                _visuals.UpdateViewVisuals(ViewDirection);
                if (_vdTarget == VelocityDriverTarget.View)
                    _velocityDriver.SetTargetDirection(ViewDirection);
            }
        }

        #endregion

        #region Triggers

        //Called by animator, and just forwards it to the unity event
        public void OnFootstep(int isOuterFoot)
        {
            onFootstep?.Invoke(isOuterFoot > 0); //yes, AnimationEvents do not support booleans, makes sense right
        }

        //Called by animator, and just forwards it to the unity event
        public void OnSwimStroke(int isOuterFoot)
        {
            onSwimStroke?.Invoke(isOuterFoot > 0); //yes, AnimationEvents do not support booleans, makes sense right
        }

        public void OnWaterEnter()
        {
            onWaterEnter?.Invoke(Mathf.Abs(_priorVelocity.y)); //nice hacky way of doing this, but no nice way for it :(
            //Debug.Log($"[PlayerController]: OnWaterEnter. isGrounded: {animator.GetBool("isGrounded")}, prev y linearvelocity: {Mathf.Abs(_priorVelocity.y)}");
            _animator.IsSwimming(true);
        }

        public void OnWaterExit()
        {
            onWaterLeave?.Invoke();
            _animator.IsSwimming(false);
        }

        public void TeleportTo(Vector3 position, bool maintainMomentum = false)
        {
            // Vector3 v = _rigidbody.linearVelocity;
            // //_rigidbody.linearVelocity = Vector3.zero;
            // _rigidbody.isKinematic = true;
            // _rigidbody.position = position;
            // _rigidbody.isKinematic = false;
            // if (maintainMomentum) _rigidbody.linearVelocity = v;

            RigidbodyInterpolation originalInterpolation = _rigidbody.interpolation;

            // 2. Disable interpolation to prevent visual smearing across the map
            _rigidbody.interpolation = RigidbodyInterpolation.None;

            // 3. Directly assign physics position and rotation
            _rigidbody.MovePosition(position);

            // Optionally clear forces so it doesn't carry velocity over
            if (!maintainMomentum)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }

            // 4. Force Unity to immediately update the internal physics engine state
            Physics.SyncTransforms();

            // 5. Restore the original interpolation on the next physics step
            _rigidbody.interpolation = originalInterpolation;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            InputVector = new Vector3(moveInput.x, 0f, moveInput.y);
            onMove?.Invoke(InputVector);
        }

        public void IsSleeping(bool isSleeping) => _visuals.gameObject.SetActive(!isSleeping);

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.started
            && Player.Instance.playerData.sprintUnlocked
            && Player.Instance.energy.energy > 0.01f)
            {
                _animator.IsSprinting(true);

                onSprint?.Invoke();
            }
            else if (context.canceled)
                _animator.IsSprinting(false);

        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.started
            && Player.Instance.playerData.attackUnlocked
            && Player.Instance.energy.energy > Player.Instance.attackEnergy)
            {
                _animator.IsAttacking(true);
                onAttack?.Invoke();
            }
            else if (context.canceled) _animator.IsAttacking(false);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                int wingLevel = Player.Instance.playerData.wingLevel;
                float energy = Player.Instance.energy.energy;
                if (wingLevel == 1 && energy > Player.Instance.hopEnergy)
                {
                    _animator.IsJumping(true);
                    onJump?.Invoke();
                }
                else if (wingLevel >= 2 && energy > 0.01f)
                {
                    _animator.IsFlying(true);
                    onJump?.Invoke();
                }
            }
            else if (context.canceled)
            {
                _animator.IsJumping(false);
                _animator.IsFlying(false);
            }
        }

        #endregion

        #region  Physics 

        private Vector3 _externalForce = Vector3.zero;
        public float LastGroundedHeight { get; private set; } = 0;

        private enum VelocityDriverTarget { Manual, Input, View }
        private VelocityDriverTarget _vdTarget = VelocityDriverTarget.Manual;

        void FixedUpdate()
        {
            _rigidbody.AddForce(_externalForce, ForceMode.Acceleration);
            UpdateGrounded();

            _priorVelocity = _currentVelocity;
            _currentVelocity = _rigidbody.linearVelocity;
        }


        private Collider[] groundColliders;
        private Vector3 _groundOffset = new(0, 0.5f, 0);
        private void UpdateGrounded()
        {
            groundColliders = Physics.OverlapBox(transform.position - _groundOffset, new(0.1f, 0.1f, 0.1f), Quaternion.identity, physicsColliders);
            if (0 < groundColliders.Length && groundColliders.Any(c => !c.isTrigger))
            {
                if (!_animator._animator.GetBool("isGrounded"))
                {
                    onLand?.Invoke();
                }
                _animator.IsGrounded(true);
                LastGroundedHeight = transform.position.y;
            }
            else
                _animator.IsGrounded(false);
        }

        #endregion

        #region API

        public void Reset() // Resets the acceleration, gravity, and locomotion to their defaults.
        {
            _externalForce = Vector3.zero;
            SetGravity(true);
            ResetLocomotion();
        }
        public void SetGravity(bool useGravity) => _rigidbody.useGravity = useGravity;
        public void SetImpulse(Vector3 vector) => _rigidbody.AddForce(vector, ForceMode.VelocityChange);
        public void SetExternalForce(Vector3 vector) => _externalForce = vector;


        #region Locomotion API

        public void SetManualLocomotion(Vector3 targetVelocity, float acceleration)
        {
            _vdTarget = VelocityDriverTarget.Manual;
            SetLocomotion(InputVector, targetVelocity.magnitude, acceleration);
        }
        public void SetInputLocomotion(float speed, float acceleration)
        {
            _vdTarget = VelocityDriverTarget.Input;
            SetLocomotion(InputVector, speed, acceleration);
        }
        public void SetViewLocomotion(float speed, float acceleration)
        {
            _vdTarget = VelocityDriverTarget.View;
            SetLocomotion(ViewDirection, speed, acceleration);
        }
        private void SetLocomotion(Vector3 direction, float speed, float acceleration)
        {
            _velocityDriver.enabled = true;
            _velocityDriver.SetTargetDirection(direction);
            _velocityDriver.SetTargetSpeed(speed);
            _velocityDriver.SetAcceleration(acceleration);
            _velocityDriver.SetAffectMode(VelocityDriver.AffectedAxesMode.NormalPlane);
            _velocityDriver.SetAffectNormal(transform.up);
            _velocityDriver.SetNormalTracksTarget(false);
        }
        private void ResetLocomotion()
        {
            _velocityDriver.Reset();
            _velocityDriver.enabled = false;
        }

        #endregion

        #endregion
    }
}
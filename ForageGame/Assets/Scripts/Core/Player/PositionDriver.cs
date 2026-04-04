// using Unity.Mathematics;
// using UnityEngine;
// using UnityEngine.Rendering.Universal;

// namespace TDK.Physics3DSystem
// {
//     // We look at how fast we are traveling in a given direction and adjust our velocity along that towards our target.
//     [RequireComponent(typeof(Rigidbody))]
//     public class PositionDriver : MonoBehaviour
//     {
//         [SerializeField] private Vector3 _targetVelocity = Vector3.zero;
//         [SerializeField] private float _acceleration = 0;

//         private Rigidbody _rigidbody;

//         void Awake()
//         {
//             _rigidbody = GetComponent<Rigidbody>();
//         }

//         void FixedUpdate()
//         {
//             _rigidbody.AddForce(
//                 Vector3.MoveTowards(
//                     Vector3.zero,
//                     _normalizeDirectionAlongEnabledAxes ?
//                         LockVector(_velocity * _direction - _rigidbody.linearVelocity, _enabledAxes) :
//                         LockVector(_velocity * _direction - _rigidbody.linearVelocity, _enabledAxes),
//                     Time.fixedDeltaTime * _acceleration),
//                 ForceMode.VelocityChange);
//         }

//         private Vector3 LockVector(this Vector3 vector, bool3 useAxes)
//         {
//             vector.x = useAxes.x ? vector.x : 0;
//             vector.y = useAxes.y ? vector.y : 0;
//             vector.z = useAxes.z ? vector.z : 0;
//             return vector;
//         }

//         private void UpdateGrounded()
//         {
//             if (Physics.SphereCast(transform.position, 0.2f, -transform.up, out RaycastHit hit, 0.6f)
//         && hit.collider.gameObject.layer != playerLayer)
//             {
//                 animator.SetBool("isGrounded", true);
//                 animator.SetBool("airDashed", false);
//                 LastGroundedHeight = transform.position.y;
//             }
//             else
//                 animator.SetBool("isGrounded", false);
//         }

// #endregion

//         #region Set Functions

//         public void Reset() // Resets the acceleration, gravity, and locomotion to their defaults.
//         {
//             externalAcceleration = Vector3.zero;
//             SetGravity(true);
//             LM_Disable();
//             LT_Disable();
//             animator.ResetTrigger("attack");
//             animator.ResetTrigger("jump");
//         }

//         public void SetGravity(bool useGravity) => Rigidbody.useGravity = useGravity;

//         public void SetImpulse(Vector3 vector) => Rigidbody.AddForce(vector, ForceMode.VelocityChange);

//         #region Locomotion Setting

//         public void LM_Set(bool3 enabledAxes, Vector3 targetVelocity, float acceleration)
//         {
//             LM_EnabledAxes = enabledAxes;
//             LM_TargetVelocity = targetVelocity;
//             LM_Acceleration = acceleration;
//         }
//         public void LM_Disable() => LM_Set(new(false, false, false), Vector3.zero, 0);
//         public void LM_Update(Vector3 targetVelocity) => LM_TargetVelocity = targetVelocity;

//         private void LT_Set(LocomotionTrack mode, float velocityScaleFactor, float acceleration)
//         {
//             LT_Mode = mode;
//             LT_VelocityScaleFactor = velocityScaleFactor;
//             LT_Acceleration = acceleration;
//         }
//         public void LT_Disable() => LT_Set(LocomotionTrack.None, 1, 0);
//         public void LT_TrackInput(float targetVelocity, float acceleration) => LT_Set(LocomotionTrack.Input, targetVelocity, acceleration);
//         public void LT_TrackView(float targetVelocity, float acceleration) => LT_Set(LocomotionTrack.View, targetVelocity, acceleration);

//         #endregion

//         #endregion
//     }
// }
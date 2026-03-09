using Unity.Mathematics;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    public Rigidbody Rigidbody { get; private set; }
    private Animator animator;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
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
                animator.SetBool("isMoving", true);
            }
            else
                animator.SetBool("isMoving", false);
        }
    }
    private Vector3 _viewDirection = Vector3.right;
    public Vector3 ViewDirection
    {
        get => _viewDirection;
        set
        {
            _viewDirection = Vector3.Normalize(value);
            Player.Instance.visuals.UpdateViewVisuals();
        }
    }

    #endregion

    #region Triggers

    public void TeleportTo(Vector3 position, bool maintainMomentum)
    {
        Vector3 v = Rigidbody.linearVelocity;
        Rigidbody.isKinematic = true;
        Rigidbody.position = position;
        Rigidbody.isKinematic = false;
        Rigidbody.linearVelocity = v;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        InputVector = new Vector3(moveInput.x, 0f, moveInput.y);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started
        && Player.Instance.playerData.dashUnlocked
        && Player.Instance.energy.energy > Player.Instance.dashEnergy)
            animator.SetBool("run", true);
        else if (context.canceled)
            animator.SetBool("run", false);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started
        && Player.Instance.playerData.attackUnlocked
        && Player.Instance.energy.energy > Player.Instance.attackEnergy)
            animator.SetTrigger("attack");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            int wingLevel = Player.Instance.playerData.wingLevel;
            float energy = Player.Instance.energy.energy;
            if (wingLevel == 1 && energy > Player.Instance.hopEnergy) animator.SetTrigger("jump");
            else if (wingLevel >= 2 && energy > 0.1f) animator.SetBool("fly", true);
        }
        else if (context.canceled)
            animator.SetBool("fly", false);
    }

    #endregion

    #region  Physics 

    public Vector3 externalAcceleration = Vector3.zero;
    public float LastGroundedHeight { get; private set; } = 0;

    private bool3 LM_EnabledAxes = new(false, false, false);
    private Vector3 LM_TargetVelocity = Vector3.zero;
    public float LM_Acceleration = 0;

    private enum LocomotionTrack { None, Input, View }
    private LocomotionTrack LT_Mode = LocomotionTrack.None;
    private float LT_VelocityScaleFactor = 1;
    public float LT_Acceleration = 0;

    void FixedUpdate()
    {
        Rigidbody.AddForce(externalAcceleration, ForceMode.Acceleration);
        ApplyLocomotion(LM_EnabledAxes, LM_TargetVelocity, LM_Acceleration);    // Manual Locomotion
        switch (LT_Mode)                                                        // Tracking Locomotion
        {
            case LocomotionTrack.None:
                break;
            case LocomotionTrack.Input:
                ApplyLocomotion(new(true, false, true), InputVector * LT_VelocityScaleFactor, LT_Acceleration);
                break;
            case LocomotionTrack.View:
                ApplyLocomotion(new(true, false, true), ViewDirection * LT_VelocityScaleFactor, LT_Acceleration);
                break;
        }
        UpdateGrounded();
    }

    private void ApplyLocomotion(bool3 enabledAxes, Vector3 targetVelocity, float acceleration)
    {
        Rigidbody.AddForce(
        Vector3.MoveTowards(
            Vector3.zero,
            GetLockedVector(
                enabledAxes,
                targetVelocity - Rigidbody.linearVelocity),
            Time.fixedDeltaTime * acceleration),
        ForceMode.VelocityChange);
    }

    private Vector3 GetLockedVector(bool3 useVector, Vector3 vector)
    {
        Vector3 result = Vector3.zero;
        result.x = useVector.x ? vector.x : 0;
        result.y = useVector.y ? vector.y : 0;
        result.z = useVector.z ? vector.z : 0;
        return result;
    }

    private void UpdateGrounded()
    {
        if (Physics.SphereCast(transform.position, 0.2f, -transform.up, out RaycastHit hit, 0.6f)
    && hit.collider.gameObject.layer != playerLayer)
        {
            animator.SetBool("isGrounded", true);
            animator.SetBool("airDashed", false);
            LastGroundedHeight = transform.position.y;
        }
        else
            animator.SetBool("isGrounded", false);
    }

    #endregion

    #region Set Functions

    public void Reset() // Resets the acceleration, gravity, and locomotion to their defaults.
    {
        externalAcceleration = Vector3.zero;
        SetGravity(true);
        LM_Disable();
        LT_Disable();
        animator.ResetTrigger("attack");
        animator.ResetTrigger("jump");
    }

    public void SetGravity(bool useGravity) => Rigidbody.useGravity = useGravity;

    public void SetImpulse(Vector3 vector) => Rigidbody.AddForce(vector, ForceMode.VelocityChange);

    #region Locomotion Setting

    public void LM_Set(bool3 enabledAxes, Vector3 targetVelocity, float acceleration)
    {
        LM_EnabledAxes = enabledAxes;
        LM_TargetVelocity = targetVelocity;
        LM_Acceleration = acceleration;
    }
    public void LM_Disable() => LM_Set(new(false, false, false), Vector3.zero, 0);
    public void LM_Update(Vector3 targetVelocity) => LM_TargetVelocity = targetVelocity;

    private void LT_Set(LocomotionTrack mode, float velocityScaleFactor, float acceleration)
    {
        LT_Mode = mode;
        LT_VelocityScaleFactor = velocityScaleFactor;
        LT_Acceleration = acceleration;
    }
    public void LT_Disable() => LT_Set(LocomotionTrack.None, 1, 0);
    public void LT_TrackInput(float targetVelocity, float acceleration) => LT_Set(LocomotionTrack.Input, targetVelocity, acceleration);
    public void LT_TrackView(float targetVelocity, float acceleration) => LT_Set(LocomotionTrack.View, targetVelocity, acceleration);

    #endregion

    #endregion
}

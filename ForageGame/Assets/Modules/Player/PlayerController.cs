using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] public SpriteRenderer sprite;
    private CharacterController characterController;
    private Animator animator;

    [SerializeField] public float groundFriction = 5;
    [SerializeField] public float airFriction = 5;




    void Awake()
    {
        characterController = GetComponent<CharacterController>();
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
                Player.Instance.animator.SetBool("isMoving", true);
            }
            else
                Player.Instance.animator.SetBool("isMoving", false);
        }
    }
    private Vector3 _viewDirection = Vector3.right;
    public Vector3 ViewDirection
    {
        get => _viewDirection;
        set
        {
            _viewDirection = Vector3.Normalize(value);
            animator.SetFloat("FacingDirection", Mathf.Clamp01(_viewDirection.z));
            sprite.flipX = _viewDirection.x > 0;
        }
    }

    #endregion

    #region Triggers

    public void OnMove(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        Vector2 moveInput = context.ReadValue<Vector2>();
        InputVector = new Vector3(moveInput.x, 0f, moveInput.y);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        if (context.action.WasPressedThisFrame())
            Player.Instance.animator.SetBool("dash", true);
        if (context.action.WasReleasedThisFrame())
            Player.Instance.animator.SetBool("dash", false);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        if (context.action.WasPressedThisFrame() && Player.Instance.playerData.canAttack)
            Player.Instance.animator.SetBool("attack", true);
        if (context.action.WasReleasedThisFrame())
            Player.Instance.animator.SetBool("attack", false);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        if (context.action.WasPressedThisFrame())
        {
            if (Player.Instance.playerData.canFlutter) Player.Instance.animator.SetBool("flutter", true);
            else if (Player.Instance.playerData.canHop) Player.Instance.animator.SetBool("hop", true);
        }
        if (context.action.WasReleasedThisFrame())
        {
            Player.Instance.animator.SetBool("hop", false);
            Player.Instance.animator.SetBool("flutter", false);
        }
    }

    #endregion

    #region  Physics 


    public bool3 useAcceleration = new(true, true, true);
    public bool3 useVelocity = new(true, true, true);
    public Vector3 externalAcceleration = Vector3.zero;
    public Vector3 externalImpulse = Vector3.zero;
    public bool useGravity = true;
    public bool3 useMomentum = new(true, true, true);

    public bool3 useLocomotion = new(false, false, false);
    public Vector3 locomotionTargetVelocity = Vector3.zero;
    public float locomotionAcceleration = 0;

    public bool3 useFriction = new(true, false, true);
    public float friction = 0;

    public float LastGroundedHeight { get; private set; } = 0;
    public Vector3 Velocity { get; private set; } = Vector3.zero;

    public readonly Vector3 gravity = -9.81f * Vector3.up;

    void Update()
    {
        UpdatePhysics();
    }

    private void UpdatePhysics()
    {
        Vector3 a = Vector3.zero;
        a += externalAcceleration;                                              // Apply external forces
        a += useGravity ? gravity : Vector3.zero;                               // Apply gravity
        a += friction * GetLockedVector(useFriction, -Velocity.normalized);     // Apply friction
        a = GetLockedVector(useAcceleration, a);

        Vector3 v = Vector3.zero;
        v += GetLockedVector(useMomentum, Velocity);                            // Apply momentum
        v += a * Time.deltaTime;                                                // Apply acceleration
        v += GetLocomotionChangeInVelocity();                                   // Apply locomotion
        v += externalImpulse;                                                   // Apply external impulse
        externalImpulse = Vector3.zero;                                         // Reset external impulse
        v = GetLockedVector(useVelocity, v);

        Vector3 priorPos = transform.position;
        characterController.Move(v * Time.deltaTime);
        Velocity = (transform.position - priorPos) / Time.deltaTime;

        UpdateGrounded();
    }

    private Vector3 GetLocomotionChangeInVelocity()
    {
        Vector3 LTargetVelocity = GetLockedVector(useLocomotion, locomotionTargetVelocity);
        Vector3 LVelocity = GetLockedVector(useLocomotion, Velocity);
        Vector3 LLocomotionResultVelocity = Vector3.MoveTowards(LVelocity, LTargetVelocity, Time.deltaTime * locomotionAcceleration);
        return LLocomotionResultVelocity - Velocity;
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
        if (Physics.SphereCast(transform.position, 0.5f, -transform.up, out RaycastHit hit, 0.6f)
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

    #region Settings Helper Functions

    public void ApplyDefaultSettings()
    {
        // Reset to idle state settings
        useAcceleration = new(true, true, true);
        useVelocity = new(true, false, true);
        externalAcceleration = Vector3.zero;
        externalImpulse = Vector3.zero;
        useGravity = true;
        useMomentum = new(true, true, true);
        useLocomotion = new(false, false, false);
        locomotionTargetVelocity = Vector3.zero;
        locomotionAcceleration = 0;
        useFriction = new(true, false, true);
        friction = 1;
    }

    public void ApplyMoveSettings(bool isGrounded, float a, float f)
    {
        useAcceleration = new(true, true, true);
        externalAcceleration = Vector3.zero;
        externalImpulse = Vector3.zero;
        useGravity = true;
        useLocomotion = new(true, false, true);
        locomotionTargetVelocity = Vector3.zero;
        locomotionAcceleration = a;
        useFriction = new(true, false, true);
        friction = f;

        if (isGrounded)
        {
            useVelocity = new(true, false, true);
            useMomentum = new(true, false, true);
        }
        else // TODO: add swimming (currently only for the air)
        {
            useVelocity = new(true, true, true);
            useMomentum = new(true, true, true);
        }
    }

    public void ApplyDashSettings()
    {
        // Dash settings (ie. Dash, Attack)
        useAcceleration = new(true, true, true);
        useVelocity = new(true, false, true);
        externalAcceleration = Vector3.zero;
        externalImpulse = Vector3.zero;
        useGravity = false;
        useMomentum = new(true, false, true);
        useLocomotion = new(false, false, false);
        locomotionTargetVelocity = Vector3.zero;
        locomotionAcceleration = 0;
        useFriction = new(true, false, true);
        friction = 1;
    }

    #endregion
}

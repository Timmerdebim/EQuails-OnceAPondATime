using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    public Rigidbody Rigidbody { get; private set; }
    private Animator animator;

    [SerializeField] public float groundFriction = 5;

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
            animator.SetTrigger("dash");
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

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {

        }
    }

    #endregion

    #region Interact



    #endregion

    #region  Physics 

    public Vector3 externalAcceleration = Vector3.zero;
    public bool useGravity = true;

    public bool3 useLocomotion = new(false, false, false);
    public Vector3 locomotionTargetVelocity = Vector3.zero;
    public float locomotionAcceleration = 0;

    public float LastGroundedHeight { get; private set; } = 0;

    void FixedUpdate()
    {
        Rigidbody.AddForce(externalAcceleration, ForceMode.Acceleration);
        Rigidbody.useGravity = useGravity;
        ApplyFriction();
        ApplyLocomotion();

        UpdateGrounded();
    }

    private void ApplyFriction()
    {
        if (animator.GetBool("isGrounded"))
        {
            // if (Rigidbody.linearVelocity.magnitude < 0.1f) // (fake) Static friction moment be like :0  [This is to help "snap" the player to a stop]
            //      Rigidbody.AddForce(GetLockedVector(new(true, false, true), -Rigidbody.linearVelocity), ForceMode.VelocityChange);
            // else
            if (Rigidbody.linearVelocity.magnitude > 0.01f)
                Rigidbody.AddForce(groundFriction * GetLockedVector(new(true, false, true), -Rigidbody.linearVelocity).normalized);
        }
    }

    private void ApplyLocomotion()
    {
        Rigidbody.AddForce(
            Vector3.MoveTowards(
                Vector3.zero,
                GetLockedVector(
                    useLocomotion,
                    locomotionTargetVelocity - Rigidbody.linearVelocity),
                Time.fixedDeltaTime * locomotionAcceleration),
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

    #region Settings Helper Functions

    public void ApplyDefaultSettings()
    {
        // Reset to idle state settings
        externalAcceleration = Vector3.zero;
        useGravity = true;
        useLocomotion = new(false, false, false);
        locomotionTargetVelocity = Vector3.zero;
        locomotionAcceleration = 0;
    }

    public void ApplyMoveSettings(float a)
    {
        externalAcceleration = Vector3.zero;
        useGravity = true;
        useLocomotion = new(true, false, true);
        locomotionTargetVelocity = Vector3.zero;
        locomotionAcceleration = a;
    }

    public void ApplyDashSettings()
    {
        // Dash settings (ie. Dash, Attack)
        externalAcceleration = Vector3.zero;
        useGravity = false;
        useLocomotion = new(false, false, false);
        locomotionTargetVelocity = Vector3.zero;
        locomotionAcceleration = 0;
    }

    #endregion
}

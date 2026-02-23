using DG.Tweening;
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


    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        priorPosition = transform.position;

    }

    // ------------ Move & View ------------

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


    // ------------ Triggers ------------

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

    // ------------ Physics ------------
    [HideInInspector] public float lastGroundHeight { get; private set; } = 0;
    [HideInInspector] public Vector3 velocity; // We set the velocity and force like this so that we can apply it correctly once per fixed update
    [HideInInspector] public float V_impulse;
    [HideInInspector] public float V_acceleration;
    [HideInInspector] public bool useGravity;
    [HideInInspector] public bool useVericalMomentum;
    public float gravity;

    private float a = 0;
    private float dv = 0;
    private Vector3 v = Vector3.zero;
    private Vector3 dx = Vector3.zero;
    private Vector3 priorPosition;

    void Update()
    {
        a = V_acceleration;
        if (useGravity) a += gravity;
        dv = a * Time.deltaTime + V_impulse;
        if (useVericalMomentum) dv += v.y;
        v = velocity + Vector3.up * dv;
        dx = v * Time.deltaTime;
        characterController.Move(dx);

        // Calculate actual dx and v
        dx = transform.position - priorPosition;
        v = dx / Time.deltaTime;
        priorPosition = transform.position;

        V_impulse = 0;
        if (Physics.SphereCast(transform.position, 0.5f, -transform.up, out RaycastHit hit, 0.6f)
            && hit.collider.gameObject.layer != playerLayer)
        {
            animator.SetBool("isGrounded", true);
            animator.SetBool("airDashed", false);
            lastGroundHeight = transform.position.y;
        }
        else
            animator.SetBool("isGrounded", false);
    }

    public void ResetSettings()
    {
        velocity = Vector3.zero;
        V_acceleration = 0;
        V_impulse = 0;
        useGravity = true;
        useVericalMomentum = false;
    }
}

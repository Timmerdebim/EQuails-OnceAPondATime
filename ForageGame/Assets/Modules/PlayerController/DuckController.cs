using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DuckController : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public DuckEnergy duckEnergy;
    public Rigidbody rb;

    private static readonly int IsDead = Animator.StringToHash("isDead");

    [Header("Walk")]
    public float walkMoveSpeed = 5f;
    [Header("Dash")]
    public float dashMoveSpeed = 60f;
    public float dashDuration = 0.15f;
    [Header("Hop")]
    public float hopMoveSpeed = 10f;
    public float hopImpulse = 10f;
    public float hopDuration = 10f;
    [Header("Flutter")]
    public float flutterMoveSpeed = 10f;
    public float flutterHeight = 10f;
    public float flutterSpringForce = 10f;
    public float flutterDuration = 10f;
    [Header("Attack")]
    public float attackMoveSpeed = 10f;
    public float attackDuration = 10f;


    [Header("Misc")]
    public LayerMask interactionLayer;
    public TrailRenderer trailRenderer;
    public ParticleSystem hitParticleRenderer;



    public enum DashType
    {
        none,
        dash,
        throughDash
    }

    public DashType dashType = DashType.throughDash;

    public BoxCollider hitboxCollider;

    public bool interactInput = false;

    //Interaction
    public PlayerInteract playerInteract;

    private void Awake()
    {
        // Get components on this GameObject
        if (playerInteract == null) TryGetComponent(out playerInteract);
        if (duckEnergy == null) TryGetComponent(out duckEnergy);
        if (duckEnergy == null)
            Debug.LogError("DuckEnergy component not found on this character. Please add one.", this);

        if (hitboxCollider == null)
            hitboxCollider = GetComponentInChildren<BoxCollider>();

        hitboxCollider.enabled = false;
        trailRenderer.emitting = false;

        if (hitParticleRenderer != null)
        {
            hitParticleRenderer.Stop();
            hitParticleRenderer.Clear(); // Use Clear() instead of setting time to 0
        }
    }

    public void Update()
    {
        // Check death using the energy component's public property
        if (duckEnergy != null && duckEnergy.currentMaxEnergy <= 0)
            animator.SetBool(IsDead, true);
    }

    public Vector2 _viewDirection { get; private set; }
    private Vector3 targetVelocity;

    public void PhysicsVelocity(float magnitude, Vector2 direction)
    {
        // Direction is a normalized Vector2 (typically the looking direction)
        targetVelocity = new Vector3(direction.x, 0, direction.y).normalized * magnitude;
    }
    public void PhysicsGravity(bool isActive)
    {
        rb.useGravity = isActive;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = targetVelocity;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();

        if (moveInput.magnitude > 0.1f)
        {
            animator.SetBool("isMoving", true);
            _viewDirection = moveInput.normalized;
        }
        else
            animator.SetBool("isMoving", false);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame())
            animator.SetTrigger("dash");
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame())
            animator.SetTrigger("attack");
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        interactInput = context.action.IsPressed();
    }

    public void OnFlutter(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame())
            animator.SetBool("flutter", true);
        if (context.action.WasReleasedThisFrame())
            animator.SetBool("flutter", false);
    }

    public void OnHop(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame())
            animator.SetTrigger("hop");
    }
}
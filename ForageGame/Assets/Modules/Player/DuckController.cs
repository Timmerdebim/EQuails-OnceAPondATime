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

    [Header("Walk")]
    public float walkMoveSpeed = 5f;
    [Header("Dash")]
    public float dashMoveSpeed = 60f; // TOOD: dash while flutter/hop (only 1 time before reset on ground)
    public float dashEnergy = 10f;
    [Header("Hop")]
    public float hopMoveSpeed = 3f;
    public float hopImpulse = 10f;
    public float hopEnergy = 10f;
    [Header("Flutter")]
    public float flutterMoveSpeed = 3f;
    public float flutterHeight = 6f; // TODO: make fullter look at last ground height position
    public float flutterNaturalFrequency = 3f; // i aint explaining this - pick up a textbook on dampening systems or smth
    public float flutterEnergy = 10f; // this is energy per second
    [Header("Attack")]
    public float attackMoveSpeed = 40f;
    public float attackEnergy = 10f;
    [Header("Fall")]
    public float fallMoveSpeed = 3f;

    [Header("Misc")]
    public LayerMask interactionLayer;
    public TrailRenderer trailRenderer;
    public ParticleSystem hitParticleRenderer;

    public float lastGroundHeight { get; private set; } = 0;

    public enum DashType
    {
        none,
        dash,
        throughDash
    }

    public DashType dashType = DashType.throughDash;

    public Hitbox hitbox;

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

        ExitStateReset();
    }

    // ------------ PHYSICS ------------

    public Vector2 _inputDirection { get; private set; }
    public Vector2 _viewDirection { get; private set; }
    public Vector3 duckVelocity; // We set the velocity and force like this so that we can apply it correctly once per fixed update
    public Vector3 duckForce;

    public void SetDuckVelocity(Vector2 direction, float magnitude)
    {
        duckVelocity = new Vector3(direction.x, 0, direction.y) * magnitude;
        duckVelocity.y = rb.linearVelocity.y;
    }

    public void DisableGravity()
    {
        rb.useGravity = false;
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0;
        rb.linearVelocity = velocity;
    }

    void FixedUpdate()
    {
        duckVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = duckVelocity;
        rb.AddForce(duckForce, ForceMode.Force);
    }

    // ------------ INPUTS ------------

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        _inputDirection = moveInput.normalized;

        if (moveInput.magnitude > 0.1f)
        {
            animator.SetBool("isMoving", true);
            _viewDirection = _inputDirection;
        }
        else
            animator.SetBool("isMoving", false);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame() && duckEnergy.energy > dashEnergy)
            animator.SetBool("dash", true);
        if (context.action.WasReleasedThisFrame())
            animator.SetBool("dash", false);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame() && duckEnergy.energy > attackEnergy)
            animator.SetBool("attack", true);
        if (context.action.WasReleasedThisFrame())
            animator.SetBool("attack", false);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        interactInput = context.action.IsPressed();
    }

    public void OnFlutter(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame() && duckEnergy.energy > flutterEnergy)
            animator.SetBool("flutter", true);
        if (context.action.WasReleasedThisFrame())
            animator.SetBool("flutter", false);
    }

    public void OnHop(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame() && duckEnergy.energy > hopEnergy)
            animator.SetBool("hop", true);
        if (context.action.WasReleasedThisFrame())
            animator.SetBool("hop", false);
    }

    // ------------ GROUND CHECK ------------

    void OnCollisionEnter(Collision collisionInfo)
    {
        animator.SetBool("isGrounded", true);
        animator.SetBool("airDashed", false);

    }

    void OnCollisionExit(Collision collision)
    {
        animator.SetBool("isGrounded", false);
        lastGroundHeight = transform.position.z;
    }

    // ------------ ANIMATOR CLUNK ------------

    public void ExitStateReset()
    {
        if (hitParticleRenderer != null)
        {
            hitParticleRenderer.Stop();
            hitParticleRenderer.Clear(); // Use Clear() instead of setting time to 0
        }

        if (interactInput) playerInteract?.StopInteract();
        duckForce = new Vector3(0, 0, 0);
        SetDuckVelocity(_viewDirection, 0);
        rb.useGravity = true;
        hitbox.gameObject.SetActive(false);
        trailRenderer.emitting = false;
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class DuckController : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public DuckEnergy duckEnergy;
    public CharacterController characterController;
    public Hitbox hitbox;
    public LayerMask interactionLayer;
    public TrailRenderer trailRenderer;
    public ParticleSystem hitParticleRenderer;
    public PlayerInteract playerInteract;

    [Header("Abilities")]
    public bool canHop;
    public bool canFlutter;
    public bool canAttack;
    [Header("Energy Requirements")]
    public float dashEnergy = 10f;
    public float hopEnergy = 10f;
    public float flutterEnergy = 10f; // this is energy per second
    public float attackEnergy = 10f;

    [Header("Misc")]
    public bool interactInput = false;

    private void Awake()
    {
        // Get components on this GameObject
        if (playerInteract == null) TryGetComponent(out playerInteract);
        if (duckEnergy == null) TryGetComponent(out duckEnergy);
        if (duckEnergy == null)
            Debug.LogError("DuckEnergy component not found on this character. Please add one.", this);
        if (characterController == null) TryGetComponent(out characterController);

        ExitStateReset();
    }

    // ------------ PHYSICS ------------

    public Vector3 _inputDirection { get; private set; }
    public Vector3 _viewDirection { get; private set; }
    public float lastGroundHeight { get; private set; } = 0;
    public Vector3 velocity; // We set the velocity and force like this so that we can apply it correctly once per fixed update
    public float V_impulse;
    public float V_acceleration;
    public bool useGravity;
    public float gravity;


    public float a;
    public float dv = 0;
    public Vector3 v;
    public Vector3 dx;

    void Update()
    {
        a = V_acceleration;
        if (useGravity) a += gravity;
        if (!characterController.isGrounded)
            dv = dv + a * Time.deltaTime + V_impulse;
        else
            dv = -0.5f + a * Time.deltaTime + V_impulse;
        v = velocity + Vector3.up * dv;
        dx = v * Time.deltaTime;
        characterController.Move(dx);

        V_impulse = 0;
        animator.SetBool("isGrounded", characterController.isGrounded);
        if (characterController.isGrounded) lastGroundHeight = transform.position.y;
        CheckEnergy(); // For knowing if we have sufficient energy
    }
    private void CheckEnergy()
    {
        animator.SetBool("dashEnergy", (dashEnergy < duckEnergy.energy));
        animator.SetBool("hopEnergy", (hopEnergy < duckEnergy.energy));
        animator.SetBool("flutterEnergy", (flutterEnergy < duckEnergy.energy));
        animator.SetBool("attackEnergy", (attackEnergy < duckEnergy.energy));
    }

    public void SetAbility(string ability, bool canAbility)
    {
        // Abilities: "Hop" "Flutter"
        animator.SetBool("can" + ability, canAbility);
    }

    // ------------ INPUTS ------------

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        moveInput = moveInput.normalized;
        _inputDirection = new Vector3(moveInput.x, 0f, moveInput.y);

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
        if (context.action.WasPressedThisFrame())
            animator.SetBool("dash", true);
        if (context.action.WasReleasedThisFrame())
            animator.SetBool("dash", false);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame() && canAttack)
            animator.SetBool("attack", true);
        if (context.action.WasReleasedThisFrame())
            animator.SetBool("attack", false);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        interactInput = context.action.IsPressed();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame())
        {
            if (canFlutter) animator.SetBool("flutter", true);
            else if (canHop) animator.SetBool("hop", true);
        }
        if (context.action.WasReleasedThisFrame())
        {
            animator.SetBool("hop", false);
            animator.SetBool("flutter", false);
        }
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
        velocity = Vector3.zero;
        V_acceleration = 0;
        V_impulse = 0;
        useGravity = true;
        hitbox.gameObject.SetActive(false);
        trailRenderer.emitting = false;
    }
}
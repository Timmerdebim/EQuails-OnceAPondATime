using System;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerData
{
    public Vector3 spawnPosition = new Vector3(0, 10, 0);
    public bool canHop = false; // replace with story flags?
    public bool canFlutter = false; // replace with story flags?
    public bool canAttack = false; // replace with story flags?
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Energy))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInteract))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Components")]
    public Animator animator { get; private set; }
    public Energy energy { get; private set; }
    public CharacterController characterController { get; private set; }
    public PlayerInteract playerInteract { get; private set; }

    [SerializeField] public SpriteRenderer sprite;
    [SerializeField] public Hitbox hitbox;
    [SerializeField] public TrailRenderer trailRenderer;
    [SerializeField] private ParticleSystem hitParticleRenderer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask interactionLayer;

    [Header("Abilities")]
    [SerializeField] public PlayerData playerData;
    [Header("Energy Requirements")]
    [SerializeField] public float dashEnergy = 10f;
    [SerializeField] public float hopEnergy = 10f;
    [SerializeField] public float flutterEnergy = 10f; // this is energy per second
    [SerializeField] public float attackEnergy = 10f;

    [Header("Misc")]
    [SerializeField] public bool interactInput = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        priorPosition = transform.position;

        // Get components on this GameObject
        animator = GetComponent<Animator>();
        energy = GetComponent<Energy>();
        characterController = GetComponent<CharacterController>();
        playerInteract = GetComponent<PlayerInteract>();

        ExitStateReset();
    }

    public void SetData(PlayerData data)
    {
        playerData = data;
    }

    public PlayerData GetData()
    {
        playerData.spawnPosition = transform.position;
        return playerData;
    }

    // ------------ Setting Functions ------------

    public void SetInputDirection(Vector3 inputDirection)
    {
        _inputDirection = inputDirection;
    }

    public void SetViewDirection(Vector3 viewDirection)
    {
        _viewDirection = viewDirection;
    }

    // ------------ PHYSICS ------------

    [HideInInspector] public Vector3 _inputDirection { get; private set; }
    [HideInInspector] public Vector3 _viewDirection { get; private set; }
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
        {
            animator.SetBool("isGrounded", false);
        }
        CheckEnergy(); // For knowing if we have sufficient energy
    }
    private void CheckEnergy()
    {
        animator.SetBool("dashEnergy", (dashEnergy < energy.energy));
        animator.SetBool("hopEnergy", (hopEnergy < energy.energy));
        animator.SetBool("flutterEnergy", (flutterEnergy < energy.energy));
        animator.SetBool("attackEnergy", (attackEnergy < energy.energy));
    }

    public void SetAbility(string ability, bool canAbility)
    {
        // Abilities: "Hop" "Flutter"
        animator.SetBool("can" + ability, canAbility);
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
        useVericalMomentum = false;

        animator.SetBool("dash", false);
        animator.SetBool("attack", false);
        animator.SetBool("hop", false);
        // animator.SetBool("flutter", false);
    }

    //Helper function, can also be used for cutscenes or something
    public void FaceTarget(Transform target)
    {
        animator.SetFloat("FacingDirection", Mathf.Clamp01(target.position.z - transform.position.z));
        sprite.flipX = target.position.x > transform.position.x;
    }
}
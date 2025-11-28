using System;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class DuckController : MonoBehaviour, ICharacterController {
    [Header("Components")] public KinematicCharacterMotor motor;
    public Animator animator;
    public DuckEnergy duckEnergy; // Reference to the new DuckEnergy component

    private static readonly int IsDead = Animator.StringToHash("isDead");

    [Header("Movement")] public bool gravityEnabled = true;

    [Header("Walking")] public float moveSpeed = 5f;
    public float orientationSharpness = 100f;

    [Header("Abilities")] public float dashSpeed = 60f;
    public float dashTime = 0.15f;

    [Header("Misc")] public List<Collider> ignoredColliders = new List<Collider>();
    public Transform cameraFollowPoint;
    public LayerMask interactionLayer;
    public TrailRenderer trailRenderer;
    public ParticleSystem hitParticleRenderer;

    public enum AttackType {
        none,
        light,
        heavy
    }

    public bool attacking = false;
    public AttackType attackType = AttackType.none;
    public float lightAttackTime = 0.2f;
    public float heavyAttackTime = 0.5f;

    public enum DashType {
        none,
        dash,
        throughDash
    }

    public DashType dashType = DashType.throughDash;

    public BoxCollider hitboxCollider;

    // state
    public Vector2 velocity = Vector2.zero;
    public Quaternion rotation = Quaternion.identity;

    // inputs
    public Vector2 moveInputVector = Vector2.zero;
    public float? timeSinceDashInput = null;
    public bool interactInput = false;

    //Interaction
    public PlayerInteract playerInteract;

    // SECTION: COLLISIONS
    private readonly Collider[] _probedColliders = new Collider[8];

    private void Awake() {
        // Assign the characterController to the motor
        motor.CharacterController = this;

        // Get components on this GameObject
        if (playerInteract == null) TryGetComponent(out playerInteract);
        if (duckEnergy == null) TryGetComponent(out duckEnergy);
        if (duckEnergy == null) {
            Debug.LogError("DuckEnergy component not found on this character. Please add one.", this);
        }

        if (hitboxCollider == null) {
            hitboxCollider = GetComponentInChildren<BoxCollider>();
        }

        hitboxCollider.enabled = false;
        trailRenderer.emitting = false;

        if (hitParticleRenderer != null) {
            hitParticleRenderer.Stop();
            hitParticleRenderer.Clear(); // Use Clear() instead of setting time to 0
        }
    }

    public void Update() {
        // Check death using the energy component's public property
        if (duckEnergy != null && duckEnergy.currentMaxEnergy <= 0) {
            animator.SetBool(IsDead, true);
        }
    }

    void Reset() {
        // This function is called in the editor when the component is added or reset.
        // It's a good place to automatically find required components.
        motor = GetComponent<KinematicCharacterMotor>();
        animator = GetComponent<Animator>();
        duckEnergy = GetComponent<DuckEnergy>();
        playerInteract = GetComponent<PlayerInteract>();
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is called before the character begins its movement update
    /// </summary>
    public void BeforeCharacterUpdate(float deltaTime) {
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its rotation should be right now. 
    /// This is the ONLY place where you should set the character's rotation
    /// </summary>
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime) {
        currentRotation = rotation;
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its velocity should be right now. 
    /// This is the ONLY place where you can set the character's velocity
    /// </summary>
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) {
        // 1. Apply planar velocity (X and Z)
        currentVelocity = new Vector3(velocity.x, currentVelocity.y, velocity.y);
    
        // 2. Apply gravity
        Vector3 gravityVec = Physics.gravity * deltaTime;
        if (gravityEnabled) {
            currentVelocity += gravityVec;
        }
        // 3. Clamp to terminal velocity (only on downward velocity)
        currentVelocity.y = Mathf.Min(currentVelocity.y, 0f);
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is called after the character has finished its movement update
    /// </summary>
    public void AfterCharacterUpdate(float deltaTime) {
        int colliders = motor.CharacterOverlap(motor.TransientPosition, motor.TransientRotation, _probedColliders,
            interactionLayer, QueryTriggerInteraction.Collide);
        for (int i = 0; i < colliders; i++) {
            Collider col = _probedColliders[i];
        }

    }

    public void PostGroundingUpdate(float deltaTime) {
        animator.SetBool("grounded", motor.GroundingStatus.FoundAnyGround);
        Debug.Log(motor.GroundingStatus.FoundAnyGround);
    }

    public bool IsColliderValidForCollisions(Collider coll) {
        if (ignoredColliders.Count == 0) {
            return true;
        }

        return !ignoredColliders.Contains(coll);
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
        ref HitStabilityReport hitStabilityReport) {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
        ref HitStabilityReport hitStabilityReport) {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
        Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) {
    }

    protected void OnLanded() {
    }

    protected void OnLeaveStableGround() {
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider) {
    }
}
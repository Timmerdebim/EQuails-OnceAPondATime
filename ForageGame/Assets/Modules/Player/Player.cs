using System;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerData
{
    public Vector3 spawnPosition = new Vector3(0, 100, 0);
    public bool canHop = false; // replace with story flags?
    public bool canFlutter = false; // replace with story flags?
    public bool canAttack = false; // replace with story flags?
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Energy))]
[RequireComponent(typeof(PlayerInteract))]
[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Components")]
    public Animator animator { get; private set; }
    public Energy energy { get; private set; }
    public PlayerInteract playerInteract { get; private set; }
    public PlayerController playerController { get; private set; }

    [SerializeField] public Hitbox hitbox;
    [SerializeField] public TrailRenderer trailRenderer;
    [SerializeField] private ParticleSystem hitParticleRenderer;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] public SpriteRenderer sprite;

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

        // Get components on this GameObject
        animator = GetComponent<Animator>();
        energy = GetComponent<Energy>();
        playerInteract = GetComponent<PlayerInteract>();
        playerController = GetComponent<PlayerController>();

        ExitStateReset();
    }

    // ------------ Save & Load ------------

    public void SaveData(ref PlayerData data)
    {
        playerData.spawnPosition = transform.position;
        data = playerData;
    }

    public void LoadData(PlayerData data)
    {
        playerData = data;

        transform.position = playerData.spawnPosition;

        ExitStateReset();

        Debug.Log(transform.position);
    }

    void Update()
    {
        CheckEnergy(); // For knowing if we have sufficient energy
    }
    private void CheckEnergy()
    {
        animator.SetBool("dashEnergy", (dashEnergy < energy.energy));
        animator.SetBool("hopEnergy", (hopEnergy < energy.energy));
        animator.SetBool("flutterEnergy", (flutterEnergy < energy.energy));
        animator.SetBool("attackEnergy", (attackEnergy < energy.energy));
    }

    // ------------ ANIMATOR CLUNK ------------

    public void ExitStateReset()
    {
        if (hitParticleRenderer != null)
        {
            hitParticleRenderer.Stop();
            hitParticleRenderer.Clear();
        }

        hitbox.gameObject.SetActive(false);
        trailRenderer.emitting = false;

        animator.SetBool("dash", false);
        animator.SetBool("attack", false);
        animator.SetBool("hop", false);

        if (interactInput) playerInteract?.StopInteract();

        playerController.ApplyDefaultSettings();
    }

    //Helper function, can also be used for cutscenes or something
    public void FaceTarget(Transform target)
    {
        animator.SetFloat("FacingDirection", Mathf.Clamp01(target.position.z - transform.position.z));
        sprite.flipX = target.position.x > transform.position.x;
    }
}
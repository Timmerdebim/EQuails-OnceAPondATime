using System;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerUpgradeType { Attack, Lantern, Pouch, Wing }

[System.Serializable]
public class PlayerData
{
    public Vector3 spawnPosition = new Vector3(0, 100, 0);
    public int wingLevel = 0;
    public int pouchLevel = 0;
    public bool attackUnlocked = false;
    public bool dashUnlocked = false;
    public bool lanternUnlocked = false;
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

    #region Save & Load

    public void SaveData(ref PlayerData data)
    {
        playerData.spawnPosition = transform.position;
        data = playerData;
    }

    public void LoadData(PlayerData data)
    {
        playerData = data;

        playerController.TeleportTo(playerData.spawnPosition, true);

        ExitStateReset();

        Debug.Log(transform.position);
    }

    #endregion

    #region Upgrades
    public void Upgrade(PlayerUpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case PlayerUpgradeType.Attack:

                break;
            case PlayerUpgradeType.Lantern:
                break;
            case PlayerUpgradeType.Pouch:
                break;
            case PlayerUpgradeType.Wing:
                break;
        }
    }

    private void SetWingLevel(int wingLevel)
    {

    }

    #endregion

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

        animator.ResetTrigger("dash");
        animator.ResetTrigger("attack");
        animator.ResetTrigger("jump");

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
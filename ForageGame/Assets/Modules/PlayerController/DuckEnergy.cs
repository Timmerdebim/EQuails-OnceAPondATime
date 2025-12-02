using System;
using UnityEngine;
using UnityEngine.UI;

public class DuckEnergy : MonoBehaviour
{
    [Header("Energy Settings")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyRegenRate = 10f;  // energy regenerated per second
    [SerializeField] private float energyRegenDelay = 3f;  // seconds delay before regen starts

    [Header("UI Settings")]
    [Tooltip("RectTransform of the energy bar GameObject")]
    [SerializeField] private RectTransform energyFill;

    [Tooltip("RectTransform of the damage bar GameObject")]
    [SerializeField] private RectTransform damageFill;

    // Current values
    public float energy { get; private set; }
    public float damage { get; set; }

    // Derived max energy after damage is considered
    public float currentMaxEnergy { get; private set; }

    // Time tracker for energy regeneration delay
    private float timeSinceEnergyUsed;

    private void Awake()
    {
        damage = 0f;
        UpdateMaxEnergy();
        energy = currentMaxEnergy;
        timeSinceEnergyUsed = energyRegenDelay; // Start ready to regenerate

        // Just a safety check so you don't forget to assign in Inspector
        if (energyFill == null)
            Debug.LogWarning("Energy Bar RectTransform not assigned.", this);
        if (damageFill == null)
            Debug.LogWarning("Damage Bar RectTransform not assigned.", this);
    }

    private void Update()
    {
        RegenerateEnergy();
        UpdateEnergyBar();
    }

    private void RegenerateEnergy()
    {
        if (energy < currentMaxEnergy)
        {
            timeSinceEnergyUsed += Time.deltaTime;
            if (timeSinceEnergyUsed >= energyRegenDelay)
            {
                energy += energyRegenRate * Time.deltaTime;
                energy = Mathf.Min(energy, currentMaxEnergy);
            }
        }
    }

    // Attempts to use energy. Returns true if successful.
    // Resets the regen timer if energy is used.
    public bool UseEnergy(float amount)
    {
        if (amount <= 0)
            return true;

        if (energy < amount)
        {
            energy = 0f;
            return false;
        }

        energy -= amount;
        if (energy < 0f)
            energy = 0f;

        timeSinceEnergyUsed = 0f;
        return true;
    }

    // Adds energy up to the current max.
    public void AddEnergy(float amount)
    {
        if (amount <= 0) return;
        energy = Mathf.Min(energy + amount, currentMaxEnergy);
    }

    // Takes damage and updates max energy accordingly.
    public void TakeDamage(float amount)
    {
        damage += amount;
        damage = Mathf.Clamp(damage, 0f, maxEnergy);

        UpdateMaxEnergy();

        // Clamp current energy to the new max
        energy = Mathf.Clamp(energy, 0f, currentMaxEnergy);
    }

    // Updates the cached max energy after considering damage.
    private void UpdateMaxEnergy()
    {
        currentMaxEnergy = maxEnergy - damage;
    }

    // Updates the UI bars positions based on current energy and damage.
    // Assumes the full width is 100 units (adjust if your bar width differs).
    private void UpdateEnergyBar()
    {
        if (energyFill != null)
        {
            // Move energy bar left as energy decreases (anchored at right)
            float energyGap = 0.005f;
            if (damage < 0.1f) energyGap = 0f;
            float energyX = Mathf.Max(0, energy / maxEnergy - energyGap);
            energyFill.sizeDelta = new Vector2(energyX, 0.8f);
        }

        if (damageFill != null)
        {
            // Move damage bar right as damage increases (anchored at left)
            float energyGap = 0.005f;
            if (currentMaxEnergy < 0.1f) energyGap = 0f;
            float damageX = Mathf.Max(0, damage / maxEnergy - energyGap);
            damageFill.sizeDelta = new Vector2(damageX, 0.8f);
        }
    }
}
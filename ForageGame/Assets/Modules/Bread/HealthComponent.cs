using JetBrains.Annotations;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    
    [CanBeNull] public IHitHandler hitHandler;
    
    public ParticleSystem hitParticles;

    public float iFramesDuration = 0.5f; // Invincibility frames duration in seconds
    private float iFramesTimer = 0f;

    public HealthComponent(int maxHealth, IHitHandler hitHandler = null) {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        this.hitHandler = hitHandler;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        if (hitParticles) {
            hitParticles.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // update invincibility timer
        if (iFramesTimer > 0f) {
            iFramesTimer -= Time.deltaTime;
            if (iFramesTimer < 0f) {
                iFramesTimer = 0f;
            }
        }
    }
    
    public void Hit(int damage) {
        // update invincibility timer
        if (iFramesTimer > 0f) {
            // Debug.Log(gameObject.name + " is invincible and took no damage.");
            return; // currently in invincibility frames
        } else {
            iFramesTimer = iFramesDuration; // reset invincibility timer
        }
        currentHealth -= damage;
        
        // assume hit particles are burst at time 0
        if (hitParticles) {
            hitParticles.time = 0f;
            hitParticles.Play();
        }

        // hitHandler?.OnHit(damage);

        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + currentHealth);
        if (currentHealth <= 0) {
            Die();
        }
    }
    
    void Die() {
        Debug.Log(gameObject.name + " died.");
        // call owner's death function if it exists
        // var deathHandler = GetComponent<IDeathHandler>();
        // if (deathHandler != null) {
        //     deathHandler.OnDeath();
        // }
        // else {
        //     Destroy(gameObject);
        // }
    }
    
    public bool isDead() {
        return currentHealth <= 0;
    }
}

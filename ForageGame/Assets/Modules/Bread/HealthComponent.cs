using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public float iFramesDuration = 0.5f; // Invincibility frames duration in seconds
    private float iFramesTimer = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
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
        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + currentHealth);
        if (currentHealth <= 0) {
            Die();
        }
    }
    
    void Die() {
        Debug.Log(gameObject.name + " died.");
        // play death animation, sound, etc.
        Destroy(gameObject);
    }
}

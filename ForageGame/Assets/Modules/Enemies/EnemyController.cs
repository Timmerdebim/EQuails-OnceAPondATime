using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public NavMeshAgent navMeshAgent;
    public SpriteRenderer spriteRenderer;

    [Header("Roam")]
    public float roamSpeed = 5f;
    public float roamRadius = 5f;
    public bool freeRoam = false; // if false, will be confined to the radius, if true they can go anywhere
    public Vector3 roamCenter; // Center of non-free roaming
    [Header("Attack")]
    public float attackSpeed = 5f;
    public float attackRadius = 5f;
    public float attackWindUpDuration = 0.7f;
    public float attackLungeDuration = 0.3f;
    public BoxCollider attackHitbox;
    [Header("Chase")]
    public float chaseSpeed = 5f;
    [Header("Search")]
    public float searchSpeed = 5f;

    // Tracking
    public DuckController player;
    public Vector3 lastSeenPlayerPos;


    void Awake()
    {
        currentHealth = maxHealth;
        hitParticles.Stop();
    }

    // A centralized utility function to set the NavMeshAgent's destination.
    // This ensures all properties are set correctly every time.
    // <param name="destination">The world-space position to move to.</param>
    // <param name="speed">The movement speed to use.</param>
    public void SetNavDestination(Vector3 destination, float speed)
    {
        // --- Safety Checks ---
        if (!navMeshAgent.enabled || !navMeshAgent.isOnNavMesh)
        {
            Debug.LogWarning("BigGoose cannot set destination - NavMeshAgent is disabled or not on a NavMesh.");
            return;
        }

        // --- Update Agent Properties ---
        navMeshAgent.speed = speed;

        // --- Set Path ---
        navMeshAgent.SetDestination(destination);

        // --- Start Moving ---
        // It's important to un-stop the agent after giving it a new path.
        navMeshAgent.isStopped = false;
    }

    // Stops all NavMeshAgent movement and pathfinding immediately.
    public void StopNavMovement()
    {
        if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath(); // Clears the current path
        }
    }

    // ------------ HEALTH ------------

    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    public ParticleSystem hitParticles;

    public void Hit(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + currentHealth);

        // assume hit particles are burst at time 0
        if (hitParticles)
        {
            hitParticles.time = 0f;
            hitParticles.Play();
        }

        if (currentHealth <= 0)
            animator.SetBool("isDead", true);
    }
}
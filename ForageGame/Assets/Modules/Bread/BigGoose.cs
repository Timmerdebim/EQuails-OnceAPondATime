using UnityEngine;
using UnityEngine.AI;

public class BigGoose : MonoBehaviour {
    private static readonly int ClosestDuckDist = Animator.StringToHash("closestDuckDist");

    public Animator animator;
    public HealthComponent healthComponent;
    public NavMeshAgent navMeshAgent;
    public BoxCollider attackHitbox;
    
    public float chaseSpeed = 5f;
    public float walkSpeed = 2f;
    
    // tracking
    public float closestDuckDist = float.MaxValue;
    public DuckController closestDuck = null;
    
    public Vector3 velocity = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;
    
    [Header("Tuning")]
    [Tooltip("How quickly the goose turns to face its movement direction. Lower is faster.")]
    public float rotationSmoothTime = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        healthComponent = GetComponent<HealthComponent>();
        if (healthComponent == null) {
            Debug.LogError("Bread: No HealthComponent found on " + gameObject.name);
        }
        attackHitbox.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        // geese chase ducks
        float frameClosestDuckDist = float.MaxValue;
        DuckController frameClosestDuck = null;
        foreach (DuckController duck in FindObjectsByType<DuckController>(FindObjectsSortMode.None)) {
            float dist = Vector3.Distance(duck.transform.position, transform.position);
            if (dist < frameClosestDuckDist) {
                frameClosestDuckDist = dist;
                frameClosestDuck = duck;
            }
        }
        closestDuckDist = frameClosestDuckDist;
        closestDuck = frameClosestDuck;
        animator.SetFloat(ClosestDuckDist, closestDuckDist);
        // print
        // Debug.Log("closestDuckDist: " + closestDuckDist);
        
        // apply velocity and rotation
        // transform.position += velocity * Time.deltaTime;
        if (!navMeshAgent.updateRotation)
        {
            // Smoothly interpolate from the current rotation to the target rotation set by the state machine.
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime / rotationSmoothTime);
        }
    }
    
    /// <summary>
    /// A centralized utility function to set the NavMeshAgent's destination.
    /// This ensures all properties are set correctly every time.
    /// </summary>
    /// <param name="destination">The world-space position to move to.</param>
    /// <param name="speed">The movement speed to use.</param>
    /// <param name="allowRotation">Should the agent automatically rotate to face its path? Set to false for strafing.</param>
    public void SetNavDestination(Vector3 destination, float speed, bool allowRotation = true)
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

    /// <summary>
    /// Stops all NavMeshAgent movement and pathfinding immediately.
    /// </summary>
    public void StopNavMovement()
    {
        if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath(); // Clears the current path
        }
    }
}
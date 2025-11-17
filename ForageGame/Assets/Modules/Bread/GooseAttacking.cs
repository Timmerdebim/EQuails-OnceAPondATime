using UnityEngine;

public class GooseAttacking : StateMachineBehaviour
{
    [Header("Attack Timings")]
    [Tooltip("How long the goose waits and stares at the target before lunging.")]
    public float windUpDuration = 0.7f;
    [Tooltip("How long the actual lunge and attack lasts.")]
    public float lungeDuration = 0.3f;
    [Tooltip("How long the goose is stuck in recovery after the lunge.")]
    public float cooldownDuration = 1.0f;

    [Header("Attack Properties")]
    [Tooltip("How fast the goose moves forward during the lunge.")]
    public float lungeSpeed = 15f;
    
    // --- NEW PROPERTIES FOR COLLISION ---
    [Header("Lunge Collision")]
    [Tooltip("The layers that the goose will collide with during its lunge (e.g., Walls, Obstacles).")]
    public LayerMask collisionLayerMask;
    [Tooltip("The radius of the goose for collision detection. Should be about half the goose's width.")]
    public float collisionRadius = 0.5f;
    // ------------------------------------

    // Private state variables
    private BigGoose goose;
    private Transform gooseTransform;
    private float timer;
    private RaycastHit hitInfo; // Stores info about what the spherecast hits

    private enum AttackPhase { WindUp, Lunge, Cooldown }
    private AttackPhase currentPhase;

    // OnStateEnter is called when a transition starts
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        goose = animator.GetComponent<BigGoose>();
        gooseTransform = animator.transform;
        if (goose == null)
        {
            Debug.LogError("GooseAttacking: No BigGoose found on " + animator.gameObject.name);
            return;
        }

        goose.StopNavMovement();
        goose.navMeshAgent.updateRotation = false; 
        goose.attackHitbox.enabled = false;

        timer = 0f;
        currentPhase = AttackPhase.WindUp;
        
        if (goose.closestDuck != null)
        {
            Vector3 directionToDuck = (goose.closestDuck.transform.position - gooseTransform.position).normalized;
            goose.rotation = Quaternion.LookRotation(new Vector3(directionToDuck.x, 0, directionToDuck.z));
        }
    }

    // OnStateUpdate is called on each Update frame
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (goose == null || goose.closestDuck == null) return;

        timer += Time.deltaTime;

        switch (currentPhase)
        {
            case AttackPhase.WindUp:
                ProcessWindUp();
                break;
            case AttackPhase.Lunge:
                ProcessLunge();
                break;
            case AttackPhase.Cooldown:
                // Do nothing, wait for animator transition.
                break;
        }
    }

    private void ProcessWindUp()
    {
        Vector3 directionToDuck = (goose.closestDuck.transform.position - gooseTransform.position).normalized;
        goose.rotation = Quaternion.LookRotation(new Vector3(directionToDuck.x, 0, directionToDuck.z));

        if (timer >= windUpDuration)
        {
            timer = 0f;
            currentPhase = AttackPhase.Lunge;
            goose.attackHitbox.enabled = true;
        }
    }

    private void ProcessLunge()
    {
        // --- REWRITTEN LUNGE LOGIC WITH COLLISION DETECTION ---
        
        // 1. Calculate how far we INTEND to move this frame.
        float distanceToMove = lungeSpeed * Time.deltaTime;

        // 2. Perform a SphereCast to see if we'll hit anything on the designated layers.
        bool willCollide = Physics.SphereCast(
            gooseTransform.position, 
            collisionRadius, 
            gooseTransform.forward, 
            out hitInfo, 
            distanceToMove, 
            collisionLayerMask
        );

        // 3. Move based on the result of the cast.
        if (willCollide)
        {
            // If we hit something, only move up to the point of contact.
            gooseTransform.position += gooseTransform.forward * hitInfo.distance;
            // Instantly stop the lunge and go into cooldown.
            EndLunge();
        }
        else
        {
            // If the path is clear, move the full distance.
            gooseTransform.position += gooseTransform.forward * distanceToMove;
        }
        
        // --- END OF NEW LOGIC ---

        // Check if the lunge duration has ended
        if (timer >= lungeDuration)
        {
            EndLunge();
        }
    }
    
    /// <summary>
    /// Helper function to cleanly end the lunge phase and enter cooldown.
    /// </summary>
    private void EndLunge()
    {
        timer = 0f;
        currentPhase = AttackPhase.Cooldown;
        goose.attackHitbox.enabled = false;
    }

    // OnStateExit is called when a transition ends
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (goose != null)
        {
            goose.attackHitbox.enabled = false;
            goose.navMeshAgent.updateRotation = true; 
        }
    }
}
using UnityEngine;
using UnityEngine.AI;

public class GooseIdle : StateMachineBehaviour
{
    [Tooltip("The radius around the goose to search for a new walk target.")]
    public float walkRadius = 10f;

    private BigGoose goose;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        goose = animator.GetComponent<BigGoose>();
        if (goose == null)
        {
            Debug.LogError("GooseIdle: No BigGoose component found on " + animator.gameObject.name);
            return;
        }

        goose.attackHitbox.enabled = false;
        
        // Find an initial destination when entering the idle state.
        SetNewRandomDestination(animator.transform.position);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If the goose has reached its destination (or is very close), find a new one.
        if (!goose.navMeshAgent.pathPending && goose.navMeshAgent.remainingDistance <= goose.navMeshAgent.stoppingDistance)
        {
            SetNewRandomDestination(animator.transform.position);
        }
    }

    /// <summary>
    /// Finds a random point on the NavMesh and tells the goose to move there.
    /// </summary>
    void SetNewRandomDestination(Vector3 origin)
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += origin;
        NavMeshHit navMeshHit;
        
        // Use NavMesh.SamplePosition to find the closest valid point on the NavMesh.
        if (NavMesh.SamplePosition(randomDirection, out navMeshHit, walkRadius, NavMesh.AllAreas))
        {
            goose.SetNavDestination(navMeshHit.position, goose.walkSpeed);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Use the utility function to stop the goose cleanly before changing state.
        if (goose != null)
        {
            goose.StopNavMovement();
        }
    }
}
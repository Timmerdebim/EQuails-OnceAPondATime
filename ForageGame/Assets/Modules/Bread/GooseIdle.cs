using UnityEngine;
using UnityEngine.AI;

public class GooseIdle : StateMachineBehaviour {
    [Tooltip("The radius around the goose to search for a new walk target.")]
    public float walkRadius = 7f;

    private BigGoose goose;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        goose = animator.GetComponent<BigGoose>();
        if (goose == null) {
            Debug.LogError("GooseIdle: No BigGoose component found on " + animator.gameObject.name);
            return;
        }

        goose.attackHitbox.enabled = false;
        SetNewRandomDestination(animator.transform.position);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If the goose has reached its destination, find a new one.
        if (!goose.navMeshAgent.pathPending && goose.navMeshAgent.remainingDistance <= goose.navMeshAgent.stoppingDistance)
        {
            SetNewRandomDestination(animator.transform.position);
        }
    }
    
    /// <summary>
    /// Finds a random point and uses the goose's utility function to move there.
    /// </summary>
    void SetNewRandomDestination(Vector3 origin)
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += origin;

        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(randomDirection, out navMeshHit, walkRadius, NavMesh.AllAreas))
        {
            goose.SetNavDestination(navMeshHit.position, goose.walkSpeed);
            goose.velocity = goose.navMeshAgent.velocity;
            // set rotation to velocity direction
            if (goose.navMeshAgent.velocity != Vector3.zero) {
                goose.rotation = Quaternion.LookRotation(
                    new Vector3(goose.navMeshAgent.velocity.x, 0, goose.navMeshAgent.velocity.z),
                    Vector3.up
                );
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Use the utility function to stop the goose cleanly.
        if (goose != null)
        {
            goose.StopNavMovement();
        }
    }
}
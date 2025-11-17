using UnityEngine;

public class GooseChasing : StateMachineBehaviour
{
    private BigGoose goose;
    private float updatePathTimer;
    private const float UPDATE_PATH_INTERVAL = 0.25f; // Update the path 4 times per second

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        goose = animator.GetComponent<BigGoose>();
        if (goose == null)
        {
            Debug.LogError("GooseChasing: No BigGoose found on " + animator.gameObject.name);
            return;
        }

        goose.attackHitbox.enabled = false;
        // The NavMeshAgent should be allowed to control rotation during chase.
        goose.navMeshAgent.updateRotation = true; 
        
        // Reset timer and set the initial destination immediately
        updatePathTimer = 0f;
        SetChaseDestination();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (goose.closestDuck == null) return;

        // Only update the destination on a timer to save performance.
        updatePathTimer += Time.deltaTime;
        if (updatePathTimer >= UPDATE_PATH_INTERVAL)
        {
            updatePathTimer = 0f;
            SetChaseDestination();
        }
    }

    private void SetChaseDestination()
    {
        // Use the goose's utility function to set the path.
        if (goose.closestDuck != null)
        {
            goose.SetNavDestination(goose.closestDuck.transform.position, goose.chaseSpeed);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (goose != null)
        {
            goose.StopNavMovement();
        }
    }
}
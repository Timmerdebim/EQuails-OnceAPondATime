using UnityEngine;

public class GooseChasing : StateMachineBehaviour
{
    BigGoose goose;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        GameObject obj = animator.gameObject;
        goose = obj.GetComponent<BigGoose>();
        if (goose == null) {
            Debug.LogError("GooseChasing: No BigGoose found on " + obj.name);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // // move towards closestDuck
        // if (goose.closestDuck != null) {
        //     Vector3 directionToDuck = (goose.closestDuck.transform.position - goose.transform.position).normalized;
        //     goose.velocity = directionToDuck * goose.chaseSpeed;
        //     goose.rotation = Quaternion.LookRotation(
        //         new Vector3(directionToDuck.x, 0, directionToDuck.z),
        //         Vector3.up
        //     );
        // } else {
        //     goose.velocity = Vector3.zero;
        // }
        goose.navMeshAgent.SetDestination(goose.closestDuck.transform.position);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        goose.velocity = Vector3.zero;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}

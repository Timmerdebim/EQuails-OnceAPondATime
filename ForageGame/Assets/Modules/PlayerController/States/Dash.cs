using UnityEngine;

public class Dash : StateMachineBehaviour, IState {
    protected DuckController duck;
    protected Vector2 duckDir;
    protected Vector2 dashVelocity;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // get gameobject this is attached to
        GameObject obj = animator.gameObject;
        // get duck controller
        duck = obj.GetComponent<DuckController>();
        duckDir = duck.velocity;
        // if original velocity is zero, set to forward
        if (duckDir == Vector2.zero) {
            // base on duck.rotation
            var forward = duck.rotation * Vector3.forward;
            duckDir = new Vector2(forward.x, forward.z);
        }

        dashVelocity = duckDir.normalized * duck.dashSpeed;
        duck.trailRenderer.emitting = true;


        if (duck.dashType == DuckController.DashType.throughDash) {
            duck.motor.CollidableLayers.value &= ~(1 << LayerMask.NameToLayer("Throughdashable"));
            duck.trailRenderer.startColor = Color.black;
            duck.trailRenderer.endColor = Color.black;
        } else {
            duck.trailRenderer.startColor = Color.yellow;
            duck.trailRenderer.endColor = Color.yellow;
        }

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.timeSinceDashInput += Time.deltaTime;
        UpdateProperties();
    }

    public void UpdateProperties() {
        duck.velocity = dashVelocity;
        duck.rotation = Quaternion.LookRotation(
            new Vector3(dashVelocity.x, 0, dashVelocity.y),
            duck.motor.CharacterUp
        );
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.timeSinceDashInput = null;
        duck.trailRenderer.emitting = false;
        duck.motor.CollidableLayers.value |= (1 << LayerMask.NameToLayer("Throughdashable"));

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

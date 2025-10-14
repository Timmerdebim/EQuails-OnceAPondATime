using UnityEngine;

public class Idle : StateMachineBehaviour, IState {
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
        if (duck == null) {
            Debug.LogError("Idle: No DuckController found on " + obj.name);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (duck.interactInput) {
            duck.playerInteract?.Interact();
        }
        else {
            duck.playerInteract?.StopInteract();
        }
        UpdateProperties();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (duck.interactInput) { duck.playerInteract?.StopInteract(); }

    }

    
    public void UpdateProperties() {
        duck.velocity = duck.moveInputVector * duck.moveSpeed;
        
        float deltaTime = Time.deltaTime;
        if (deltaTime > 0) {
            // Smoothly interpolate from current to target look direction
            Vector3 smoothedLookInputDirection = Vector3.Slerp(duck.motor.CharacterForward,
                new Vector3(duck.moveInputVector.x, 0, duck.moveInputVector.y),
                1 - Mathf.Exp(-duck.orientationSharpness * deltaTime)).normalized;

            // Set the current rotation (which will be used by the KinematicCharacterMotor)
            if (smoothedLookInputDirection.sqrMagnitude > 0f) {
                duck.rotation = Quaternion.LookRotation(smoothedLookInputDirection, duck.motor.CharacterUp);
            }
        }
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

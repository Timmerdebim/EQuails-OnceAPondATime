using KinematicCharacterController;
using UnityEngine;

public class Flutter : StateMachineBehaviour, IState {
    protected DuckController duck;
    
    float startY = 0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        // get gameobject this is attached to
        GameObject obj = animator.gameObject;
        // get duck controller
        duck = obj.GetComponent<DuckController>();

        float jumpHeight = 4f;
        
        // move up 
        // duck.motor.HasPlanarConstraint = false;
        // duck.transform.position += new Vector3(0, 4f, 0);
        duck.motor.HasPlanarConstraint = false;
        startY = obj.transform.position.y;
        duck.motor.SetPosition(duck.transform.position + new  Vector3(0, jumpHeight, 0));
        // do not move the CameraFollowPoint child object
        duck.cameraFollowPoint.transform.localPosition = new Vector3(0, -jumpHeight, 0);
        duck.gravityEnabled = false;

        // duck.motor.rigid


    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.timeSinceDashInput += Time.deltaTime;
        UpdateProperties();
    }

    public void UpdateProperties() {
        // duck.velocity = 
        duck.velocity = MovementUtils.inputToVelocity(duck) / 2;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.gravityEnabled = true;

        // reset y
        // Vector3 pos = duck.transform.position;
        // pos.y = startY;
        // duck.motor.SetPosition(pos);
        
        // duck.motor.
        // duck.motor.
        duck.cameraFollowPoint.transform.localPosition = Vector3.zero;
        // duck.motor.HasPlanarConstraint = true;
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

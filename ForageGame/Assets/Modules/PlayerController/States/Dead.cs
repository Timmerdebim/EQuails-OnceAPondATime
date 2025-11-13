using UnityEngine;

public class Dead : StateMachineBehaviour, IState {
    protected DuckController duck;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // get gameobject this is attached to
        GameObject obj = animator.gameObject;
        // get duck controller
        duck = obj.GetComponent<DuckController>();
        if (duck == null) {
            Debug.LogError("Dead: No DuckController found on " + obj.name);
        }
        // set velocity to zero
        duck.velocity = Vector2.zero;
        // set rotation to upside down
        duck.rotation = Quaternion.identity;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // do nothing
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // do nothing
    }

    public void UpdateProperties() {
        duck.velocity = Vector2.zero;
        duck.rotation = Quaternion.identity;
    }
}
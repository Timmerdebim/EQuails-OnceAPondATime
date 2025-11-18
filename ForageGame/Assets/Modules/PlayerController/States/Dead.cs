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
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // do nothing
        UpdateProperties();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // do nothing
    }

    public void UpdateProperties() {
        duck.velocity = Vector2.zero;
        // make it spin continuously
        var time = Time.time;
        duck.rotation = Quaternion.Euler(0, 0, time * 100);
    }
}
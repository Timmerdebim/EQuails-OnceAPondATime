using UnityEngine;

public class Fall : StateMachineBehaviour
{
    protected DuckController duck;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        duck = obj.GetComponent<DuckController>();
        if (duck == null)
            Debug.LogError("Fall: No DuckController found on " + obj.name);

        duck.animator.SetBool("isBusy", true);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.PhysicsVelocity(duck.walkMoveSpeed, duck._viewDirection);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.animator.SetBool("isBusy", false);
        duck.PhysicsVelocity(0, new Vector2(0, 0));
    }
}

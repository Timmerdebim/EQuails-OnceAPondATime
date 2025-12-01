using UnityEngine;

public class Move : StateMachineBehaviour
{
    protected DuckController duck;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        duck = obj.GetComponent<DuckController>();
        if (duck == null)
            Debug.LogError("Move: No DuckController found on " + obj.name);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (duck.interactInput)
            duck.playerInteract?.Interact();
        else
            duck.playerInteract?.StopInteract();

        duck.PhysicsVelocity(duck.walkMoveSpeed, duck._viewDirection);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (duck.interactInput) duck.playerInteract?.StopInteract();
        duck.PhysicsVelocity(0, new Vector2(0, 0));
    }
}

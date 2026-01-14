using UnityEngine;

public class Move : StateMachineBehaviour
{
    protected DuckController duck;
    [SerializeField] private float moveSpeed;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        duck = obj.GetComponent<DuckController>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (duck.interactInput)
            duck.playerInteract?.Interact();
        else
            duck.playerInteract?.StopInteract();

        duck.velocity = duck._inputDirection * moveSpeed;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.ExitStateReset();
    }
}

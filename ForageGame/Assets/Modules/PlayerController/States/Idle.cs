using UnityEngine;

public class Idle : StateMachineBehaviour
{
    protected DuckController duck;

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
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (duck.interactInput) duck.playerInteract?.StopInteract();
    }
}

using UnityEngine;

public class Move : StateMachineBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float moveAcceleration = 10;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.playerController.ApplyMoveSettings(true, moveAcceleration, Player.Instance.playerController.groundFriction);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Player.Instance.interactInput)
            Player.Instance.playerInteract?.Interact();
        else
            Player.Instance.playerInteract?.StopInteract();

        Player.Instance.playerController.locomotionTargetVelocity = moveSpeed * Player.Instance.playerController.InputVector;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.ExitStateReset();
    }
}

using UnityEngine;

public class Walk : StateMachineBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float moveAcceleration = 10;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.playerController.ApplyMoveSettings(moveAcceleration);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.playerController.locomotionTargetVelocity = moveSpeed * Player.Instance.playerController.InputVector;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.ExitStateReset();
    }
}

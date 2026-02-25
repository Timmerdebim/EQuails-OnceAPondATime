using Unity.Mathematics;
using UnityEngine;

public class Fall : StateMachineBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float moveAcceleration = 10;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.playerController.ApplyMoveSettings(false, moveAcceleration, Player.Instance.playerController.airFriction);
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

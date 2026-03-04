using System;
using UnityEngine;

public class Jump : StateMachineBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float moveAcceleration = 10;
    [SerializeField] private float hopHeight;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.energy.UseEnergy(Player.Instance.hopEnergy);
        Player.Instance.playerData.hasUsedJump = true;

        Player.Instance.playerController.ApplyMoveSettings(moveAcceleration);

        Player.Instance.playerController.Rigidbody.AddForce(-Physics.gravity.normalized * Mathf.Sqrt(2 * Physics.gravity.magnitude * hopHeight), ForceMode.Impulse);
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

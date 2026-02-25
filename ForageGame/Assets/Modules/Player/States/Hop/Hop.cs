using System;
using UnityEngine;

public class Hop : StateMachineBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float moveAcceleration = 10;
    [SerializeField] private float hopHeight;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.energy.UseEnergy(Player.Instance.hopEnergy);

        Player.Instance.playerController.ApplyMoveSettings(false, moveAcceleration, Player.Instance.playerController.airFriction);

        Player.Instance.playerController.externalImpulse = Vector3.up * Mathf.Sqrt(2 * Math.Abs(Player.Instance.playerController.gravity.y) * hopHeight);
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

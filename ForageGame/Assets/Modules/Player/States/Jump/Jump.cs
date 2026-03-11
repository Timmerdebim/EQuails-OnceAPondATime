using System;
using UnityEngine;

public class Jump : StateMachineBehaviour
{
    [SerializeField] private float maxSpeed = 10;
    [SerializeField] private float acceleration = 10;
    [SerializeField] private float hopHeight;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.energy.UseEnergy(Player.Instance.hopEnergy);
        Player.Instance.playerData.hasUsedJump = true;

        Player.Instance.playerController.Reset();
        Player.Instance.playerController.LT_TrackView(maxSpeed, acceleration);
        Player.Instance.playerController.SetImpulse(-Physics.gravity.normalized * Mathf.Sqrt(2 * Physics.gravity.magnitude * hopHeight));
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.ExitStateReset();
    }
}

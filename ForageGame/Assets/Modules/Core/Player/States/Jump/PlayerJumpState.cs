using System;
using UnityEngine;

namespace TDK.PlayerSystem.States
{
    public class PlayerJumpState : StateMachineBehaviour
    {
        [SerializeField] private float maxSpeed = 10;
        [SerializeField] private float acceleration = 10;
        [SerializeField] private float hopHeight;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("isBusy", true);

            Player.Instance.energy.AddEnergy(-Player.Instance.hopEnergy);
            Player.Instance.playerData.hasUsedJump = true;
            Player.Instance.energy.SetRegenEnabled(false);

            Player.Instance.playerController.Reset();
            Player.Instance.playerController.SetInputLocomotion(maxSpeed, acceleration);
            Player.Instance.playerController.SetImpulse(-Physics.gravity.normalized * Mathf.Sqrt(2 * Physics.gravity.magnitude * hopHeight));

            PlayerSounds.Instance.PlayTakeOff();
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("isBusy", false);
            animator.SetBool("jump", false);
            Player.Instance.ExitStateReset();
        }
    }
}

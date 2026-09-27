using UnityEngine;

namespace TDK.PlayerSystem.States
{
    public class PlayerRunState : StateMachineBehaviour
    {
        [SerializeField] private float maxSpeed = 10;
        [SerializeField] private float acceleration = 10;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Player.Instance.playerController.Reset();
            Player.Instance.playerController.SetInputLocomotion(maxSpeed, acceleration);
            Player.Instance.energy.SetRegenEnabled(false);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Player.Instance.energy.AddEnergy(-Player.Instance.runEnergy * Time.deltaTime);
            // Check if still can run
            if (Player.Instance.energy.energy < 0.01f)
                animator.SetBool("run", false);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Player.Instance.ExitStateReset();
        }
    }
}
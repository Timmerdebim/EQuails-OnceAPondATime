using UnityEngine;

namespace TDK.PlayerSystem.States
{
    public class PlayerDeadState : StateMachineBehaviour
    {
        [SerializeField] private float deceleration = 30;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Player.Instance.playerController.Reset();
            Player.Instance.playerController.SetManualLocomotion(Vector3.zero, deceleration);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }
}
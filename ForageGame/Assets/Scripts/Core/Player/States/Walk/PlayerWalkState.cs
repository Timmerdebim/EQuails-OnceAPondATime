using UnityEngine;

namespace TDK.PlayerSystem.States
{
    public class PlayerWalkState : StateMachineBehaviour
    {
        [SerializeField] private float maxSpeed = 10;
        [SerializeField] private float acceleration = 10;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Player.Instance.playerController.Reset();
            Player.Instance.playerController.LT_TrackInput(maxSpeed, acceleration);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Player.Instance.ExitStateReset();
        }
    }
}
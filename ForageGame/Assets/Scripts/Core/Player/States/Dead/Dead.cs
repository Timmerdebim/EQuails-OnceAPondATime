using UnityEngine;

namespace TDK.PlayerSystem.States
{
    public class Dead : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Player.Instance.playerController.Reset();
            Player.Instance.playerController.LM_Set(new(true, false, true), Vector3.zero, 99);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }
}
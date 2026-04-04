using UnityEngine;
using UnityEngine.AI;

namespace TDK.EnemySystem.States
{
    public class EnemyConfused : StateMachineBehaviour
    {
        private EnemyController enemy;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy = animator.GetComponent<EnemyController>();
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }
}
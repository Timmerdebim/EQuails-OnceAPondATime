using System;
using UnityEngine;

namespace TDK.PlayerSystem.States
{
    public class PlayerAttackState : StateMachineBehaviour
    {
        [SerializeField] private float impulse = 10;
        [SerializeField] private float deceleration = 10;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Player.Instance.energy.UseEnergy(Player.Instance.attackEnergy);
            Player.Instance.playerData.hasUsedAttack = true;

            Player.Instance.hitbox.Reset();
            Player.Instance.hitbox.PivotTarget(Player.Instance.playerController.ViewDirection);
            Player.Instance.hitbox.gameObject.SetActive(true);

            Player.Instance.playerController.Reset();
            Player.Instance.playerController.SetGravity(false);
            Player.Instance.playerController.SetManualLocomotion(Vector3.zero, deceleration);
            Player.Instance.playerController.SetImpulse(impulse * Player.Instance.playerController.ViewDirection);
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
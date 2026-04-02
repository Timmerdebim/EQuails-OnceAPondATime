using UnityEngine;

namespace TDK.PlayerSystem.States
{
    public class Dash : StateMachineBehaviour
    {
        [SerializeField] private float impulse = 10;
        [SerializeField] private float deceleration = 10;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Player.Instance.energy.UseEnergy(Player.Instance.dashEnergy);
            Player.Instance.playerData.hasUsedDash = true;

            Player.Instance.trailRenderer.emitting = true;
            Player.Instance.trailRenderer.startColor = Color.black;
            Player.Instance.trailRenderer.endColor = Color.black;

            if (!animator.GetBool("isGrounded"))
                animator.SetBool("airDashed", true);

            Player.Instance.playerController.Reset();
            Player.Instance.playerController.SetGravity(false);
            Player.Instance.playerController.LM_Set(new(true, false, true), Vector3.zero, deceleration);
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
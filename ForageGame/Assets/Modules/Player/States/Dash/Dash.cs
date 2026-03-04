using UnityEngine;

public class Dash : StateMachineBehaviour
{
    [SerializeField] private float impulse = 10;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.energy.UseEnergy(Player.Instance.dashEnergy);
        Player.Instance.playerData.hasUsedDash = true;

        Player.Instance.trailRenderer.emitting = true;
        Player.Instance.trailRenderer.startColor = Color.black;
        Player.Instance.trailRenderer.endColor = Color.black;

        if (!animator.GetBool("isGrounded"))
            animator.SetBool("airDashed", true);

        Player.Instance.playerController.ApplyDashSettings();
        Player.Instance.playerController.Rigidbody.AddForce(impulse * Player.Instance.playerController.ViewDirection, ForceMode.Impulse);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.ExitStateReset();
    }
}

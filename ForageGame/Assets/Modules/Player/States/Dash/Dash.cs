using UnityEngine;

public class Dash : StateMachineBehaviour
{
    [SerializeField] private float moveSpeed;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;

        Player.Instance.trailRenderer.emitting = true;

        if (!animator.GetBool("isGrounded"))
            animator.SetBool("airDashed", true);


        Player.Instance.trailRenderer.startColor = Color.black;
        Player.Instance.trailRenderer.endColor = Color.black;

        Player.Instance.playerController.useGravity = false;
        Player.Instance.playerController.velocity = Player.Instance.playerController.ViewDirection * moveSpeed;

        Player.Instance.energy.UseEnergy(Player.Instance.dashEnergy);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.ExitStateReset();
    }
}

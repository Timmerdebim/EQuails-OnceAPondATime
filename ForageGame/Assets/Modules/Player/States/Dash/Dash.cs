using UnityEngine;

public class Dash : StateMachineBehaviour
{
    protected DuckController duck;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        duck = obj.GetComponent<DuckController>();

        duck.trailRenderer.emitting = true;

        if (!animator.GetBool("isGrounded"))
            animator.SetBool("airDashed", true);

        if (duck.dashType == DuckController.DashType.throughDash)
        {
            duck.trailRenderer.startColor = Color.black;
            duck.trailRenderer.endColor = Color.black;
        }
        else
        {
            duck.trailRenderer.startColor = Color.yellow;
            duck.trailRenderer.endColor = Color.yellow;
        }

        duck.DisableGravity();
        duck.SetDuckVelocity(duck._viewDirection, duck.dashMoveSpeed);

        duck.duckEnergy.UseEnergy(duck.dashEnergy);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.ExitStateReset();
    }
}

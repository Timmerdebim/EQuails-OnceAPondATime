using UnityEngine;

public class Dash : StateMachineBehaviour
{
    protected DuckController duck;
    [SerializeField] private float moveSpeed;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        duck = obj.GetComponent<DuckController>();

        duck.trailRenderer.emitting = true;

        if (!animator.GetBool("isGrounded"))
            animator.SetBool("airDashed", true);


        duck.trailRenderer.startColor = Color.black;
        duck.trailRenderer.endColor = Color.black;

        duck.useGravity = false;
        duck.velocity = duck._viewDirection * moveSpeed;

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

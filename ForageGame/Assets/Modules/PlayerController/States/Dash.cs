using UnityEngine;

public class Dash : StateMachineBehaviour
{
    protected DuckController duck;
    protected float timeSinceDashStart;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        duck = obj.GetComponent<DuckController>();

        duck.trailRenderer.emitting = true;

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

        duck.animator.SetBool("isBusy", true);
        timeSinceDashStart = 0f;
        duck.PhysicsGravity(false);
        duck.PhysicsVelocity(duck.dashMoveSpeed, duck._viewDirection);

        duck.duckEnergy.UseEnergy(10);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeSinceDashStart += Time.deltaTime;
        if (timeSinceDashStart > duck.dashDuration)
        {
            duck.animator.SetBool("isBusy", false);
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.animator.SetBool("isBusy", false);
        timeSinceDashStart = 0f;
        duck.trailRenderer.emitting = false;
        duck.PhysicsGravity(true);
        duck.PhysicsVelocity(0, new Vector2(0, 0));
    }
}

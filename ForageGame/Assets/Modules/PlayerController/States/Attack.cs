using UnityEngine;

public class Attack : StateMachineBehaviour
{
    protected DuckController duck;
    protected float timeSinceAttackStart;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        duck = obj.GetComponent<DuckController>();

        duck.animator.SetBool("isBusy", true);
        timeSinceAttackStart = 0;
        duck.hitboxCollider.enabled = true;
        duck.PhysicsVelocity(duck.attackMoveSpeed, duck._viewDirection);
        duck.PhysicsGravity(false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeSinceAttackStart += Time.deltaTime;
        if (timeSinceAttackStart > duck.attackDuration)
        {
            duck.animator.SetBool("isBusy", false);
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.animator.SetBool("isBusy", false);
        timeSinceAttackStart = 0;
        duck.hitboxCollider.enabled = false;
        duck.PhysicsVelocity(0, new Vector2(0, 0));
        duck.PhysicsGravity(true);
    }
}

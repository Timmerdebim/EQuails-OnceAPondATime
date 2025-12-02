using UnityEngine;

public class Attack : StateMachineBehaviour
{
    protected DuckController duck;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        duck = obj.GetComponent<DuckController>();

        duck.hitboxCollider.enabled = true;
        duck.SetDuckVelocity(duck._viewDirection, duck.attackMoveSpeed);
        duck.rb.useGravity = false;

        duck.duckEnergy.UseEnergy(duck.attackEnergy);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.hitboxCollider.enabled = false;
        duck.SetDuckVelocity(duck._viewDirection, 0);
        duck.rb.useGravity = true;
    }
}

using System;
using UnityEngine;

public class Attack : StateMachineBehaviour
{
    protected DuckController duck;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        duck = obj.GetComponent<DuckController>();

        duck.hitbox.Reset();
        duck.hitbox.PivotTarget(new Vector3(duck._viewDirection.x, 0, duck._viewDirection.y));
        duck.hitbox.gameObject.SetActive(true);
        duck.SetDuckVelocity(duck._viewDirection, duck.attackMoveSpeed);
        duck.DisableGravity();

        duck.duckEnergy.UseEnergy(duck.attackEnergy);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.ExitStateReset();
    }
}

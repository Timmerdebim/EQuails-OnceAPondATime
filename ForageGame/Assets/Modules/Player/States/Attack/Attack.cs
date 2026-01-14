using System;
using UnityEngine;

public class Attack : StateMachineBehaviour
{
    protected DuckController duck;

    [SerializeField] private float moveSpeed;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        duck = obj.GetComponent<DuckController>();

        duck.hitbox.Reset();
        duck.hitbox.PivotTarget(duck._viewDirection);
        duck.hitbox.gameObject.SetActive(true);
        duck.velocity = duck._viewDirection * moveSpeed;
        duck.useGravity = false;

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

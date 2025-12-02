using System;
using UnityEngine;

public class Attack : StateMachineBehaviour
{
    protected DuckController duck;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        duck = obj.GetComponent<DuckController>();

        duck.hitboxCollider.transform.localPosition = new Vector3(duck._viewDirection.x, 0, duck._viewDirection.y);
        duck.hitboxCollider.transform.localEulerAngles = new Vector3(0, Mathf.Atan2(duck._viewDirection.y, duck._viewDirection.x) * 180f / Mathf.PI, 0);
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

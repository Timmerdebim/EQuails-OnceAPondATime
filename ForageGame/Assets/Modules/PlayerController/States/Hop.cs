using System;
using KinematicCharacterController;
using UnityEngine;

public class Hop : StateMachineBehaviour
{
    protected DuckController duck;
    protected float timeSinceHopInput;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        duck = obj.GetComponent<DuckController>();

        duck.animator.SetBool("isBusy", true);
        duck.rb.AddForce(new Vector3(0, duck.hopImpulse, 0), ForceMode.Impulse);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeSinceHopInput += Time.deltaTime;
        if (timeSinceHopInput > duck.hopDuration)
        {
            duck.animator.SetBool("isBusy", false);
            return;
        }
        duck.PhysicsVelocity(duck.hopMoveSpeed, duck._viewDirection);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.animator.SetBool("isBusy", false);
        duck.PhysicsVelocity(0, new Vector2(0, 0));
    }
}

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

        duck.rb.AddForce(new Vector3(0, duck.hopImpulse, 0), ForceMode.Impulse);

        duck.duckEnergy.UseEnergy(duck.hopEnergy);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.SetDuckVelocity(duck._inputDirection, duck.hopMoveSpeed);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.ExitStateReset();
    }
}

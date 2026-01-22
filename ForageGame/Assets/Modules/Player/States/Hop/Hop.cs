using System;
using KinematicCharacterController;
using UnityEngine;

public class Hop : StateMachineBehaviour
{
    protected DuckController duck;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float hopHeight;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        duck = obj.GetComponent<DuckController>();

        duck.V_impulse = Mathf.Sqrt(2 * Math.Abs(duck.gravity) * hopHeight);

        duck.duckEnergy.UseEnergy(duck.hopEnergy);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.velocity = duck._inputDirection * moveSpeed;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.ExitStateReset();
    }
}

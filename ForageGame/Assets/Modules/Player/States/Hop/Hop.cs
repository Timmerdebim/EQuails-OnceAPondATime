using System;
using UnityEngine;

public class Hop : StateMachineBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float hopHeight;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;

        Player.Instance.useVericalMomentum = true;

        Player.Instance.V_impulse = Mathf.Sqrt(2 * Math.Abs(Player.Instance.gravity) * hopHeight);

        Player.Instance.energy.UseEnergy(Player.Instance.hopEnergy);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.velocity = Player.Instance._inputDirection * moveSpeed;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.ExitStateReset();
    }
}

using System;
using UnityEngine;

public class Attack : StateMachineBehaviour
{
    [SerializeField] private float moveSpeed;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;

        Player.Instance.hitbox.Reset();
        Player.Instance.hitbox.PivotTarget(Player.Instance._viewDirection);
        Player.Instance.hitbox.gameObject.SetActive(true);
        Player.Instance.velocity = Player.Instance._viewDirection * moveSpeed;
        Player.Instance.useGravity = false;

        Player.Instance.energy.UseEnergy(Player.Instance.attackEnergy);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.ExitStateReset();
    }
}

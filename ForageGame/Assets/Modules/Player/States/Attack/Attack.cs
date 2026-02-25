using System;
using UnityEngine;

public class Attack : StateMachineBehaviour
{
    [SerializeField] private float impulse = 10;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.energy.UseEnergy(Player.Instance.attackEnergy);

        Player.Instance.hitbox.Reset();
        Player.Instance.hitbox.PivotTarget(Player.Instance.playerController.ViewDirection);
        Player.Instance.hitbox.gameObject.SetActive(true);

        Player.Instance.playerController.ApplyDashSettings();
        Player.Instance.playerController.Rigidbody.AddForce(impulse * Player.Instance.playerController.ViewDirection, ForceMode.Impulse);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.ExitStateReset();
    }
}

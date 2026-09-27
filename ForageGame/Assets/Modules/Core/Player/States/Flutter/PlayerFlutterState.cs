using UnityEngine;

namespace TDK.PlayerSystem.States
{
    public class PlayerFlutterState : StateMachineBehaviour
    {
        private float targetHight;
        [SerializeField] private float moveSpeed = 10;
        [SerializeField] private float moveAcceleration = 10;
        [SerializeField] private float flutterHeight = 6;
        [SerializeField] private float flutterNaturalFrequency;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Player.Instance.playerData.hasUsedFly = true;
            Player.Instance.energy.SetRegenEnabled(false);

            // the flutter tax -- prevents using falling to save on energy (ie. tax them for the energy they saved while falling)
            // Player.Instance.energy.AddEnergy(-Player.Instance.flutterEnergy * Mathf.Sqrt(Mathf.Abs(2 * flutterHeight / Physics.gravity.y)));
            // disabled cause it looks meh :/

            Player.Instance.playerController.LastGroundedHeight = Mathf.Min(Player.Instance.playerController.LastGroundedHeight, Player.Instance.transform.position.y);
            targetHight = flutterHeight + Player.Instance.playerController.LastGroundedHeight;

            Player.Instance.playerController.Reset();
            Player.Instance.playerController.SetInputLocomotion(moveSpeed, moveAcceleration);
            Player.Instance.playerController.SetGravity(false);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Player.Instance.energy.AddEnergy(-Player.Instance.flutterEnergy * Time.deltaTime);
            Player.Instance.playerController.SetExternalForce(Vector3.up * (flutterNaturalFrequency * flutterNaturalFrequency * (targetHight - Player.Instance.transform.position.y) - 2 * flutterNaturalFrequency * Player.Instance.playerController._rigidbody.linearVelocity.y));

            // Check if still can fly
            if (Player.Instance.energy.energy < 0.001f)
                animator.SetBool("fly", false);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Player.Instance.ExitStateReset();
        }
    }
}
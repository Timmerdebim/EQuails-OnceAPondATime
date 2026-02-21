using KinematicCharacterController;
using UnityEngine;

public class Flutter : StateMachineBehaviour
{
    private float targetHight;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float flutterHeight;
    [SerializeField] private float flutterNaturalFrequency;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;

        targetHight = flutterHeight + Player.Instance.lastGroundHeight;
        Player.Instance.useGravity = false;
        Player.Instance.useVericalMomentum = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.energy.UseEnergy(Player.Instance.flutterEnergy * Time.deltaTime);
        Player.Instance.velocity = Player.Instance._inputDirection * moveSpeed;
        Player.Instance.V_acceleration = (flutterNaturalFrequency * flutterNaturalFrequency * (targetHight - Player.Instance.transform.position.y) - 2 * flutterNaturalFrequency * Player.Instance.characterController.velocity.y);

        // Check if still can fly
        if (Player.Instance.energy.energy < 0.001f)
            animator.SetBool("flutter", false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.ExitStateReset();
    }
}

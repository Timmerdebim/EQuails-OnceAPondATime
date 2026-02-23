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

        targetHight = flutterHeight + Player.Instance.playerController.lastGroundHeight;
        Player.Instance.playerController.useGravity = false;
        Player.Instance.playerController.useVericalMomentum = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.energy.UseEnergy(Player.Instance.flutterEnergy * Time.deltaTime);
        Player.Instance.playerController.velocity = Player.Instance.playerController.InputVector * moveSpeed;
        Player.Instance.playerController.V_acceleration = (flutterNaturalFrequency * flutterNaturalFrequency * (targetHight - Player.Instance.transform.position.y) - 2 * flutterNaturalFrequency * Player.Instance.playerController.velocity.y);

        // Check if still can fly
        if (Player.Instance.energy.energy < 0.001f)
            animator.SetBool("flutter", false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.ExitStateReset();
    }
}

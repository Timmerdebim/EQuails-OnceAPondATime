using UnityEngine;

public class Flutter : StateMachineBehaviour
{
    private float targetHight;
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float moveAcceleration = 10;
    [SerializeField] private float flutterHeight = 6;
    [SerializeField] private float flutterNaturalFrequency;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;

        targetHight = flutterHeight + Player.Instance.playerController.LastGroundedHeight;

        Player.Instance.playerController.ApplyMoveSettings(moveAcceleration);
        Player.Instance.playerController.useGravity = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.energy.UseEnergy(Player.Instance.flutterEnergy * Time.deltaTime); // TODO: fix this?
        Player.Instance.playerController.locomotionTargetVelocity = moveSpeed * Player.Instance.playerController.InputVector;
        Player.Instance.playerController.externalAcceleration = Vector3.up * (flutterNaturalFrequency * flutterNaturalFrequency * (targetHight - Player.Instance.transform.position.y) - 2 * flutterNaturalFrequency * Player.Instance.playerController.Rigidbody.linearVelocity.y);

        // Check if still can fly
        if (Player.Instance.energy.energy < 0.001f)
            animator.SetBool("fly", false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.ExitStateReset();
    }
}

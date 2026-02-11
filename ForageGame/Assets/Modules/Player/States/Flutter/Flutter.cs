using KinematicCharacterController;
using UnityEngine;

public class Flutter : StateMachineBehaviour
{
    protected DuckController duck;
    private float targetHight;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float flutterHeight;
    [SerializeField] private float flutterNaturalFrequency;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        duck = obj.GetComponent<DuckController>();

        targetHight = flutterHeight + duck.lastGroundHeight;
        duck.useGravity = false;
        duck.useVericalMomentum = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.duckEnergy.UseEnergy(duck.flutterEnergy * Time.deltaTime);
        duck.velocity = duck._inputDirection * moveSpeed;
        duck.V_acceleration = (flutterNaturalFrequency * flutterNaturalFrequency * (targetHight - duck.transform.position.y) - 2 * flutterNaturalFrequency * duck.characterController.velocity.y);

        // Check if still can fly
        if (duck.duckEnergy.energy < 0.001f)
            animator.SetBool("flutter", false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.ExitStateReset();
    }
}

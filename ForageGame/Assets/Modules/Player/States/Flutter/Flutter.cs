using KinematicCharacterController;
using UnityEngine;

public class Flutter : StateMachineBehaviour
{
    protected DuckController duck;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        duck = obj.GetComponent<DuckController>();

        duck.rb.useGravity = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.duckEnergy.UseEnergy(duck.flutterEnergy * Time.deltaTime);
        duck.SetDuckVelocity(duck._inputDirection, duck.flutterMoveSpeed);
        duck.duckForce = new Vector3(0, duck.flutterNaturalFrequency * duck.flutterNaturalFrequency * (duck.flutterHeight - duck.transform.position.y) - 2 * duck.flutterNaturalFrequency * duck.rb.linearVelocity.y, 0);

        // Check if still can fly
        if (duck.duckEnergy.energy < 0.001f)
            animator.SetBool("flutter", false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.duckForce = new Vector3(0, 0, 0);
        duck.SetDuckVelocity(duck._viewDirection, 0);
        duck.rb.useGravity = true;
    }
}

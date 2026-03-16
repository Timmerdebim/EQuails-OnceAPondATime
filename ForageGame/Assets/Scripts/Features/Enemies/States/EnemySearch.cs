using UnityEngine;
using UnityEngine.AI;

public class EnemySearch : StateMachineBehaviour
{
    private EnemyController enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<EnemyController>();

        enemy.SetNavDestination(enemy.lastSeenPlayerPos, enemy.searchSpeed);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.StopNavMovement();
    }
}
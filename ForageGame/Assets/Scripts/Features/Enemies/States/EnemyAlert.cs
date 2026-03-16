using UnityEngine;
using UnityEngine.AI;

public class EnemyAlert : StateMachineBehaviour
{
    private EnemyController enemy;
    private Vector3 targetDir;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<EnemyController>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Look at player
        targetDir = Player.Instance.transform.position - enemy.transform.position;
        enemy.spriteRenderer.flipX = targetDir.x < 0;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.ExitStateReset();
    }
}
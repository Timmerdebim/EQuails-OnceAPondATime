using UnityEngine;
using UnityEngine.AI;

public class EnemyRoam : StateMachineBehaviour
{
    private EnemyController enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<EnemyController>();

        // Finds a random point on the NavMesh and tells the goose to move there.
        Vector3 randomDirection = Random.insideUnitSphere * enemy.roamRadius;
        if (enemy.freeRoam) randomDirection += animator.transform.position;
        else randomDirection += enemy.roamCenter;
        NavMeshHit navMeshHit;
        // Use NavMesh.SamplePosition to find the closest valid point on the NavMesh.
        if (NavMesh.SamplePosition(randomDirection, out navMeshHit, 1f, NavMesh.AllAreas))
        {
            enemy.SetNavDestination(navMeshHit.position, enemy.roamSpeed);
            if ((navMeshHit.position - enemy.transform.position).x < 0) enemy.spriteRenderer.flipX = true;
            else enemy.spriteRenderer.flipX = false;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If the goose has reached its destination (or is very close), find a new one.
        if (!enemy.navMeshAgent.pathPending && enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance)
        {
            // Finds a random point on the NavMesh and tells the goose to move there.
            Vector3 randomDirection = Random.insideUnitSphere * enemy.roamRadius;
            if (enemy.freeRoam) randomDirection += enemy.roamCenter;
            else randomDirection += animator.transform.position;
            NavMeshHit navMeshHit;

            // Use NavMesh.SamplePosition to find the closest valid point on the NavMesh.
            if (NavMesh.SamplePosition(randomDirection, out navMeshHit, enemy.roamRadius, NavMesh.AllAreas))
            {
                enemy.SetNavDestination(navMeshHit.position, enemy.roamSpeed);
                if ((navMeshHit.position - enemy.transform.position).x < 0) enemy.spriteRenderer.flipX = true;
                else enemy.spriteRenderer.flipX = false;
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.StopNavMovement();
    }
}
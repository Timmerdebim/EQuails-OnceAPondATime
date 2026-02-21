using UnityEngine;

public class EnemyChase : StateMachineBehaviour
{
    private EnemyController enemy;
    private float updatePathTimer;
    private const float UPDATE_PATH_INTERVAL = 0.25f; // Update the path 4 times per second

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<EnemyController>();

        updatePathTimer = 0f;

        enemy.SetNavDestination(Player.Instance.transform.position, enemy.chaseSpeed);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Only update the destination on a timer to save performance.
        updatePathTimer += Time.deltaTime;
        if (updatePathTimer >= UPDATE_PATH_INTERVAL)
        {
            updatePathTimer = 0f;
            enemy.SetNavDestination(enemy.lastSeenPlayerPos, enemy.chaseSpeed);
        }
        if ((enemy.lastSeenPlayerPos - enemy.transform.position).x < 0) enemy.spriteRenderer.flipX = true;
        else enemy.spriteRenderer.flipX = false;

        // Try attacking
        if (Vector3.Distance(enemy.transform.position, Player.Instance.transform.position) < enemy.attackRadius)
            animator.SetTrigger("attack");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.StopNavMovement();
    }
}
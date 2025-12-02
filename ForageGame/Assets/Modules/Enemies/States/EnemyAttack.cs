using UnityEngine;

public class EnemyAttack : StateMachineBehaviour
{
    [Header("Lunge Collision")]
    [Tooltip("The layers that the goose will collide with during its lunge (e.g., Walls, Obstacles).")]
    public LayerMask collisionLayerMask;
    [Tooltip("The radius of the goose for collision detection. Should be about half the goose's width.")]
    public float collisionRadius = 0.5f;
    // ------------------------------------

    // Private state variables
    private EnemyController enemy;
    private float timer;
    private RaycastHit hitInfo; // Stores info about what the spherecast hits

    private enum AttackPhase { WindUp, Lunge, Cooldown }
    private AttackPhase currentPhase;
    private Vector3 targetDir;

    // OnStateEnter is called when a transition starts
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<EnemyController>();

        timer = 0f;
        currentPhase = AttackPhase.WindUp;
        // Look in (and lock in) correct direction
        targetDir = enemy.player.transform.position - enemy.transform.position;
        if (targetDir.x < 0) enemy.spriteRenderer.flipX = true;
        else enemy.spriteRenderer.flipX = false;
        targetDir.y = 0f;
        targetDir = targetDir.normalized;
        enemy.attackHitbox.transform.position = targetDir;
        enemy.attackHitbox.transform.eulerAngles = targetDir;
    }

    // OnStateUpdate is called on each Update frame
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        switch (currentPhase)
        {
            case AttackPhase.WindUp:
                ProcessWindUp();
                break;
            case AttackPhase.Lunge:
                ProcessLunge();
                break;
            case AttackPhase.Cooldown:
                break; // Do nothing, wait for animator transition.
        }
    }

    private void ProcessWindUp()
    {
        if (timer > enemy.attackWindUpDuration)
        {
            timer = 0f;
            currentPhase = AttackPhase.Lunge;
            // Start Lunge
            enemy.attackHitbox.enabled = true;
            enemy.navMeshAgent.enabled = false; // Disable NavMeshAgent
        }
    }

    private void ProcessLunge()
    {
        float distanceToMove = enemy.attackSpeed * Time.deltaTime;

        // Perform a SphereCast to see if we'll hit anything on the designated layers.
        bool willCollide = Physics.SphereCast(
            enemy.transform.position,
            collisionRadius,
            targetDir,
            out hitInfo,
            distanceToMove,
            collisionLayerMask
        );

        // Move based on the result of the cast.
        if (willCollide) // If we hit something, only move up to the point of contact.
            enemy.transform.position += targetDir * hitInfo.distance;
        else // If the path is clear, move the full distance.
            enemy.transform.position += targetDir * distanceToMove;

        // Check if the lunge duration has ended
        if (timer > enemy.attackLungeDuration)
        {
            timer = 0f;
            currentPhase = AttackPhase.Cooldown;
            enemy.attackHitbox.enabled = false;
            enemy.navMeshAgent.enabled = true; // ReEnable NavMeshAgent
        }
    }

    // OnStateExit is called when a transition ends
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.attackHitbox.enabled = false;
        enemy.navMeshAgent.enabled = true; // ReEnable NavMeshAgent
    }
}
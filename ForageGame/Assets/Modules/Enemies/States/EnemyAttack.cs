using UnityEngine;

public class EnemyAttack : StateMachineBehaviour
{
    [Header("Lunge Collision")]
    [Tooltip("The layers that the goose will collide with during its lunge (e.g., Walls, Obstacles).")]
    public LayerMask collisionLayerMask;
    [Tooltip("The radius of the goose for collision detection. Should be about half the goose's width.")]
    private float collisionRadius = 0.5f;
    // ------------------------------------

    // Private state variables
    private EnemyController enemy;
    private CharacterController characterController;
    private float timer;

    private enum AttackPhase { WindUp, Lunge, Cooldown }
    private AttackPhase currentPhase;
    private Vector3 targetDir;

    // OnStateEnter is called when a transition starts
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<EnemyController>();
        characterController = enemy.GetComponent<CharacterController>();

        timer = 0f;
        currentPhase = AttackPhase.WindUp;
        enemy.navMeshAgent.enabled = false; // Disable NavMeshAgent

        // Look at player
        targetDir = Player.Instance.transform.position - enemy.transform.position;
        enemy.spriteRenderer.flipX = targetDir.x < 0;
        // Set attack hitbox
        targetDir.y = 0f;
        targetDir = targetDir.normalized;
        enemy.hitBox.PivotTarget(targetDir);
    }

    // OnStateUpdate is called on each Update frame
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        switch (currentPhase)
        {
            case AttackPhase.WindUp:
                if (timer > enemy.attackWindUpDuration) EndWindUp();
                break;
            case AttackPhase.Lunge:
                if (timer > enemy.attackLungeDuration) EndLundge();
                else ProcessLunge();
                break;
            case AttackPhase.Cooldown:
                break; // Do nothing, wait for animator transition.
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.ExitStateReset();
    }

    // ------------ FUNCTIONS ------------

    private void EndWindUp()
    {
        timer = 0f;
        currentPhase = AttackPhase.Lunge;
        enemy.hitBox.Reset();
        enemy.hitBox.gameObject.SetActive(true);
    }

    private void EndLundge()
    {
        timer = 0f;
        currentPhase = AttackPhase.Cooldown;
        enemy.hitBox.gameObject.SetActive(false);
    }

    private void ProcessLunge()
    {
        characterController.Move(targetDir * enemy.attackSpeed * Time.deltaTime);
    }
}
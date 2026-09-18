using UnityEngine;
using TDK.PlayerSystem;

namespace TDK.EnemySystem.States
{
    public class EnemyAttack : StateMachineBehaviour
    {

        [SerializeField] private float _lungeStart = 1f;
        [SerializeField] private float _lungeStop = 2f;
        [SerializeField] private float _lungeSpeed = 5f;
        // [Header("Lunge Collision")]
        // [Tooltip("The layers that the goose will collide with during its lunge (e.g., Walls, Obstacles).")]
        // public LayerMask collisionLayerMask;
        // [Tooltip("The radius of the goose for collision detection. Should be about half the goose's width.")]
        // private float collisionRadius = 0.5f;
        // ------------------------------------

        // Private state variables
        private EnemyController _enemy;
        private CharacterController characterController;
        private float timer;
        private Vector3 targetDir;

        // OnStateEnter is called when a transition starts
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _enemy = animator.GetComponent<EnemyController>();
            characterController = _enemy.GetComponent<CharacterController>();

            timer = 0f;
            _enemy.navMeshAgent.enabled = false; // Disable NavMeshAgent

            // Set attack hitbox
            targetDir = _enemy._lastPlayerPos - _enemy.transform.position;
            targetDir.y = 0f;
            targetDir = targetDir.normalized;
            _enemy.SetHitboxDirection(targetDir);
        }

        // OnStateUpdate is called on each Update frame
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            timer += Time.deltaTime;

            if (timer > _lungeStart && timer < _lungeStop)
                characterController.Move(_lungeSpeed * Time.deltaTime * targetDir);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _enemy.SetHitboxInactive();
            _enemy.navMeshAgent.enabled = true;
        }
    }
}
using UnityEngine;
using TDK.PlayerSystem;

namespace TDK.EnemySystem.States
{
    public class EnemyChase : StateMachineBehaviour
    {
        private EnemyController _enemy;
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _attackRadius = 5f;
        private float _updatePathTimer = 0;
        private const float UPDATE_PATH_INTERVAL = 0.25f; // Update the path 4 times per second

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _enemy = animator.GetComponent<EnemyController>();

            _updatePathTimer = 0f;
            _enemy._popupTextbox.ShowTextbox(false);

            _enemy.GoToLastPlayerPos(_speed);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Only update the destination on a timer to save performance.
            _updatePathTimer += Time.deltaTime;
            if (_updatePathTimer >= UPDATE_PATH_INTERVAL)
            {
                _updatePathTimer = 0f;
                _enemy.GoToLastPlayerPos(_speed);
            }
            _enemy.LookAtPlayer();

            // Try attacking
            if (Vector3.Distance(_enemy.transform.position, _enemy._lastPlayerPos) < _attackRadius)
                animator.SetTrigger("attack");
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _enemy.StopNavMovement();
        }
    }
}
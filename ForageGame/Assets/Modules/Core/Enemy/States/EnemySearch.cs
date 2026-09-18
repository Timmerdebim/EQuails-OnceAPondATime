using UnityEngine;
using UnityEngine.AI;

namespace TDK.EnemySystem.States
{
    public class EnemySearch : StateMachineBehaviour
    {
        private EnemyController _enemy;
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _minSearchTime = 1f;
        [SerializeField] private float _maxSearchTime = 5f;
        private float _searchTimer = 0f;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _enemy = animator.GetComponent<EnemyController>();

            _searchTimer = 0f;
            _enemy._popupTextbox.ShowTextbox(false);

            _enemy.GoToLastPlayerPos(_speed);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _searchTimer += Time.deltaTime;
            if (_maxSearchTime < _searchTimer ||
                (_minSearchTime < _searchTimer && !_enemy.navMeshAgent.pathPending && _enemy.navMeshAgent.remainingDistance <= _enemy.navMeshAgent.stoppingDistance))
                _enemy.SetAlertLevel(0);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _enemy.StopNavMovement();
        }
    }
}
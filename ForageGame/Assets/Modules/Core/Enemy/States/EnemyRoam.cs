using UnityEngine;
using UnityEngine.AI;

namespace TDK.EnemySystem.States
{
    public class EnemyRoam : StateMachineBehaviour
    {
        private EnemyController _enemy;

        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _roamRadius = 5f;
        [SerializeField] private bool _freeRoam = false; // if false, will be confined to the radius, if true they can go anywhere
        [SerializeField] private float _upadePathInterval = 10f; // if false, will be confined to the radius, if true they can go anywhere
        private float _updatePathTimer = 0;
        private Vector3 _roamCenter; // Center of non-free roaming

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _enemy = animator.GetComponent<EnemyController>();

            _roamCenter = _enemy.transform.position;
            _updatePathTimer = 0;

            GoToRandomPosition();
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _updatePathTimer += Time.deltaTime;
            // Renew if taking too long or if already there
            if (_updatePathTimer > _upadePathInterval ||
                (!_enemy.navMeshAgent.pathPending && _enemy.navMeshAgent.remainingDistance <= _enemy.navMeshAgent.stoppingDistance))
            {
                _updatePathTimer = 0;
                GoToRandomPosition();
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _enemy.StopNavMovement();
        }

        private void GoToRandomPosition()
        {
            // Finds a random point on the NavMesh and tells the goose to move there.
            Vector3 randomPoint = Random.insideUnitSphere * _roamRadius;
            if (_freeRoam) randomPoint += _enemy.transform.position;
            else randomPoint += _roamCenter;

            // Use NavMesh.SamplePosition to find the closest valid point on the NavMesh.
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit navMeshHit, _roamRadius, NavMesh.AllAreas))
            {
                _enemy.SetNavDestination(navMeshHit.position, _speed);
                _enemy.LookAtPoint(navMeshHit.position);
            }
        }
    }
}
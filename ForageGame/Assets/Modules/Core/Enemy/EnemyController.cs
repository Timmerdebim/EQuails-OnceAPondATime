using TDK.PlayerSystem;
using TDK.SaveSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace TDK.EnemySystem
{
    public class EnemyController : MonoBehaviour, IHitHandler   //, ISaveable, ILoadable
    {
        [Header("Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] public NavMeshAgent navMeshAgent;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [SerializeField] private EnemyAudioSensor _audioSensor;
        [SerializeField] private EnemyVisionSensor _visionSensor;
        [SerializeField] private HitEffect hurtEffect;
        [SerializeField] private Hitbox _hitBox;
        [SerializeField] public PopupTextbox _popupTextbox;

        private int _alertLevel = 0;
        private int _sensorLevel = 0;
        private bool _isDead = false;
        public Vector3 _lastPlayerPos { get; private set; } = Vector3.zero;
        private Transform _player;

        public UnityEvent<float> onHurt;

        void Awake()
        {
            currentHealth = maxHealth;
            hurtEffect.Initialize(onHurt, maxHealth);
            SetAlertLevel(0);
        }

        void Start()
        {
            _player = Player.Instance.transform;
        }

        void Update()
        {
            if (_isDead) return;

            // Update Sensor Level
            if (_sensorLevel == 0)
            {
                if (_audioSensor._sensorTriggered) SetSensorLevel(1);
            }
            else if (_sensorLevel == 1)
            {
                _lastPlayerPos = _player.position;
                if (!_audioSensor._sensorTriggered) SetSensorLevel(0);
                else if (_visionSensor._sensorTriggered) SetSensorLevel(2);
            }
            else if (_sensorLevel == 2)
            {
                _lastPlayerPos = _player.position;
                if (!_visionSensor._sensorTriggered) SetSensorLevel(1);
            }
        }

        private void SetSensorLevel(int level)
        {
            // sensor level logic
            // 0 - cant hear
            // 1 - can hear
            // 2 - can see

            if (_sensorLevel == level) return;
            _sensorLevel = level;
            _visionSensor.gameObject.SetActive(_sensorLevel != 0);

            if (_sensorLevel == 2) SetAlertLevel(2);
            else if (_sensorLevel == 1) SetAlertLevel(1);
            // alert level 0 is reached by decay via the searching state code
        }

        public void SetAlertLevel(int level)
        {
            // alert level logic
            // 0 - all good
            // 1 - persuing intrest (ie. last sighting / current audio)
            // 2 - active persuit

            if (_alertLevel == level) return;

            _animator.ResetTrigger("AlertLevel0");
            _animator.ResetTrigger("AlertLevel1");
            _animator.ResetTrigger("AlertLevel2");

            if (level < _sensorLevel) level = _sensorLevel; // to prevent issues (im not gonna explain - ur just gonna need to trust me on this one)

            _alertLevel = level;

            if (level == 0)
            {
                _popupTextbox.ShowTextbox(false);
                _animator.SetTrigger("AlertLevel0");
            }
            else if (level == 1)
            {
                _popupTextbox.SetText("?");
                _popupTextbox.ShowTextbox(true);
                _animator.SetTrigger("AlertLevel1");

            }
            else if (level == 2)
            {
                _popupTextbox.SetText("!");
                _popupTextbox.ShowTextbox(true);
                _animator.SetTrigger("AlertLevel2");
            }
        }


        // A centralized utility function to set the NavMeshAgent's destination.
        // This ensures all properties are set correctly every time.
        // <param name="destination">The world-space position to move to.</param>
        // <param name="speed">The movement speed to use.</param>
        public void SetNavDestination(Vector3 destination, float speed)
        {
            // --- Safety Checks ---
            if (!navMeshAgent.enabled || !navMeshAgent.isOnNavMesh)
            {
                Debug.LogWarning("BigGoose cannot set destination - NavMeshAgent is disabled or not on a NavMesh.");
                return;
            }
            navMeshAgent.speed = speed;
            navMeshAgent.SetDestination(destination);

            // --- Start Moving ---
            // It's important to un-stop the agent after giving it a new path.
            navMeshAgent.isStopped = false;
        }

        // Stops all NavMeshAgent movement and pathfinding immediately.
        public void StopNavMovement()
        {
            if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath(); // Clears the current path
            }
        }

        // ------------ STATES FUNCTIONS ------------

        public void LookAtPlayer()
        {
            LookAtPoint(_player.position);
        }

        public void LookAtPoint(Vector3 position)
        {
            _spriteRenderer.flipX = (position - transform.position).x < 0;
        }

        private void SetHitboxState(bool active)
        {
            if (active == _hitBox.gameObject.activeSelf) return;
            _hitBox.Reset();
            _hitBox.gameObject.SetActive(active);
        }
        public void SetHitboxActive() => SetHitboxState(true); // for the animator
        public void SetHitboxInactive() => SetHitboxState(false);

        public void SetHitboxDirection(Vector3 direction)
        {
            _hitBox.PivotTarget(direction.normalized);
        }

        public void GoToLastPlayerPos(float speed)
        {
            SetNavDestination(_lastPlayerPos, speed);
        }

        // ------------ HEALTH ------------

        [Header("Health")]
        public float maxHealth = 100f;
        public float currentHealth;

        public void Hit(float damage)
        {
            currentHealth -= damage;
            Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + currentHealth);
            onHurt.Invoke(damage);
            if (currentHealth <= 0)
            {
                _isDead = true;
                _animator.SetBool("isDead", true);
                SetSensorLevel(0);
                SetAlertLevel(0);
            }
        }

        // #region Save & Load
        // // ------------ SAVE & LOAD ------------

        // public void SaveData(ref WorldSaveData data)
        // {
        //     throw new System.NotImplementedException();
        // }

        // public void LoadData(WorldSaveData data)
        // {
        //     throw new System.NotImplementedException();
        // }

        // #endregion
    }
}
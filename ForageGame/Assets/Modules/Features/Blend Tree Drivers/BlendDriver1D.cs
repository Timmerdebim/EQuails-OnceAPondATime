using UnityEngine;
using UnityEngine.Events;

namespace TDK.BlendTreeDrivers
{
    [RequireComponent(typeof(Animator))]
    public class BlendDriver1D : MonoBehaviour
    {
        private Animator _animator;
        [SerializeField] private string parameterName = "Blend";
        [SerializeField] private float transitionSpeed = 1f;

        private int _paramHash;
        private float _current;
        private float _target;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _paramHash = Animator.StringToHash(parameterName);
            _current = _animator.GetFloat(_paramHash);
            _target = _animator.GetFloat(_paramHash);
        }

        public void SetTarget(float target) => _target = target;

        private void Update()
        {
            if (Mathf.Approximately(_current, _target)) return;

            _current = Mathf.MoveTowards(_current, _target, transitionSpeed * Time.deltaTime);
            _animator.SetFloat(_paramHash, _current);
        }
    }
}
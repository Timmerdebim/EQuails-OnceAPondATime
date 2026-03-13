using Assets.Modules.Interaction;
using TDK.GadgetSystem.Runtime;
using UnityEngine;

namespace TDK.Gadgets
{
    [RequireComponent(typeof(OutputPort))]
    [RequireComponent(typeof(Animator))]
    public class PowerSwitchController : MonoBehaviour, IInteractable
    {
        private OutputPort _outputPower;
        private Animator _animator;

        [SerializeField] private bool _isActive = false;

        void Awake()
        {
            _outputPower = GetComponent<OutputPort>();
            _animator = GetComponent<Animator>();
        }

        void Start()
        {
            _outputPower.SetSignal(_isActive);
            _animator.SetBool("isActive", _isActive);
        }

        public void Interact()
        {
            _isActive = !_isActive;
            _outputPower.SetSignal(_isActive);
            _animator.SetBool("isActive", _isActive);
        }

        public void Focus()
        {
            throw new System.NotImplementedException();
        }

        public void Unfocus()
        {
            throw new System.NotImplementedException();
        }
    }
}
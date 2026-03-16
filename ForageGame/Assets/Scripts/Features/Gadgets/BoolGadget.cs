using Assets.Modules.Interaction;
using TDK.PortSystem;
using UnityEngine;
using UnityEngine.Events;

namespace TDK.Gadgets
{
    public class BoolGadget : MonoBehaviour
    {
        public UnityEvent OnActivate;
        public UnityEvent OnDeactivate;

        [SerializeField] private bool _initialState = false;
        public bool Locked = false;
        private bool _state;
        public bool State
        {
            get => _state;
            private set
            {
                if (Locked) return;
                _state = value;
                if (_state) OnActivate.Invoke();
                else OnDeactivate.Invoke();
            }
        }
        void Start() => Initialize(_initialState, false);
        public void Initialize(bool state, bool locked)
        {
            _state = state;
            if (_state) OnActivate.Invoke();
            else OnDeactivate.Invoke();
            Locked = locked;
        }
        public void ToggleState() => State = !State;
        public void SetState(bool state) => State = state;
    }
}

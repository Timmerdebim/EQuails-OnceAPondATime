using Assets.Modules.Interaction;
using TDK.PortSystem;
using UnityEngine;
using UnityEngine.Events;

namespace TDK.Gadgets
{
    public class SwitchController : MonoBehaviour
    {
        public UnityEvent OnSwitchedOn;
        public UnityEvent OnSwitchedOff;

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
                if (_state) OnSwitchedOn.Invoke();
                else OnSwitchedOff.Invoke();
            }
        }

        void Start() { State = _initialState; }

        public void ToggleState() => SetState(!State);

        public void SetState(bool state) { if (!Locked) State = state; }
    }
}
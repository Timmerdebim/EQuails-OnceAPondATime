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
        [SerializeField] private bool _state = false;

        void Start()
        {
            _state = _initialState;
            if (_state) OnSwitchedOn.Invoke();
            else OnSwitchedOff.Invoke();
        }

        public void ToggleState() => SetState(!_state);

        public void SetState(bool state)
        {
            if (_state == state) return;
            _state = state;
            if (_state) OnSwitchedOn.Invoke();
            else OnSwitchedOff.Invoke();
        }
    }
}
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace TDK.Gadgets
{
    public class DoorController : MonoBehaviour
    {
        public UnityEvent OnDoorOpened;
        public UnityEvent OnDoorClosed;

        [SerializeField] private bool _initialStateOpen = false;
        [SerializeField] private bool _state = false;

        void Start()
        {
            _state = _initialStateOpen;
            if (_state) OnDoorOpened.Invoke();
            else OnDoorClosed.Invoke();
        }

        public void ToggleState() => SetState(!_state);

        public void SetState(bool state)
        {
            if (_state == state) return;
            _state = state;
            if (_state) OnDoorOpened.Invoke();
            else OnDoorClosed.Invoke();
        }
    }
}
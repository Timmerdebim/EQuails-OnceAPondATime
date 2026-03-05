using Project.Ports;
using UnityEngine;
using DG.Tweening;
using Project.CustomTweenUtils;
using System;

namespace Project.Signals.Targets
{
    public class SlidingDoor : MonoBehaviour
    {
        public InputPort<Unit> _togglePort;
        public InputPort<bool> _setPort;

        [SerializeField] private bool startOpen = false;
        [SerializeField] private bool singnalUse = false;
        [SerializeField] private Transform doorTransform;

        [SerializeField] private TweenTransformOptions openOptions;
        [SerializeField] private TweenTransformOptions closedOptions;

        private enum State { Open, Closing, Closed, Opening, None }
        private State currentState = State.None;

        private Sequence sequence;

        void Awake()
        {
            _togglePort = new InputPort<Unit>(ToggleDoor);
            _setPort = new InputPort<bool>(SetDoorState);
        }

        void Start()
        {
            SetDoorState(startOpen, true);
        }

        private void ToggleDoor(Unit unit)
        {
            if (currentState == State.Open) CloseDoor();
            else if (currentState == State.Closed) OpenDoor();
        }

        private void SetDoorState(bool setToOpen)
        {
            if (setToOpen) OpenDoor();
            else CloseDoor();
        }

        private void SetDoorState(bool setToOpen, bool direct = false)
        {
            if (setToOpen) OpenDoor(direct);
            else CloseDoor(direct);
        }

        private void OpenDoor(bool direct = false)
        {
            if (direct)
            {
                sequence?.Kill();
                doorTransform.SetPositionAndRotation(openOptions.target.position, openOptions.target.rotation);
                currentState = State.Open;
                return;
            }
            if (currentState == State.Open || currentState == State.Opening) return;
            currentState = State.Opening;
            sequence?.Kill();
            sequence = DOTween.Sequence()
            .Append(doorTransform.DOMoveTransform(openOptions))
            .OnComplete(() => currentState = State.Open);
        }

        private void CloseDoor(bool direct = false)
        {
            if (direct)
            {
                sequence?.Kill();
                doorTransform.SetPositionAndRotation(closedOptions.target.position, closedOptions.target.rotation);
                currentState = State.Closed;
                return;
            }
            if (currentState == State.Closed || currentState == State.Closing) return;
            currentState = State.Closing;
            sequence?.Kill();
            sequence = DOTween.Sequence()
            .Append(doorTransform.DOMoveTransform(closedOptions))
            .OnComplete(() => currentState = State.Closed);
        }

        void OnDestroy()
        {
            sequence?.Kill();
            // PortSystem.UnregisterPorts(this);
        }
    }
}

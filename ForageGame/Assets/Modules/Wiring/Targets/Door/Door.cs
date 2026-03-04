using UnityEngine;

namespace Project.Signals.Targets
{
    [RequireComponent(typeof(Animation))]
    public class Door : SignalTarget, ISignalTarget
    {
        private Animation anim;
        [SerializeField] private AnimationClip openingDoorClip;
        [SerializeField] private AnimationClip closingDoorClip;
        void Awake() => anim = GetComponent<Animation>();
        [SerializeField] private bool isOpen = false;
        [SerializeField] private bool signalUse = false;

        private void ToggleDoor()
        {
            if (anim.isPlaying) return;
            SetDoorOpen(!isOpen);
        }

        public void SetDoorOpen(bool toOpen)
        {
            if (anim.isPlaying) return;
            if (isOpen == toOpen) return;
            isOpen = toOpen;

            if (toOpen) anim.clip = openingDoorClip;
            else anim.clip = closingDoorClip;
            anim.Play();
        }

        // public void ReceiveSignal<Bool>(Bool signal) => SetDoorOpen(signal);
        public void ReceiveSignal<EmptySignal>(EmptySignal signal) => ToggleDoor();

    }
}

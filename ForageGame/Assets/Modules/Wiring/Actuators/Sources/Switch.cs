using Project.Signals.Actuators;
using UnityEngine;

namespace Project.Signals.Sources
{
    // A switch has two states; on and off
    [RequireComponent(typeof(Animation))]
    public class Switch : SignalSource, IActuator
    {
        private Animation anim;
        void Awake() => anim = GetComponent<Animation>();
        protected bool isOpen = true;
        public void OnTrigger()
        {
            if (anim.isPlaying) return;
            anim.Play();
            isOpen = !isOpen;
            SignalManager.Instance?.SendSignalFrom(isOpen, this);
        }
        public virtual void Focus() { }
        public virtual void Unfocus() { }
    }
}


using Project.Signals.Actuators;
using UnityEngine;

namespace Project.Signals.Sources
{
    // A switch has two states; on and off
    public class Switch : SignalSource, IActuator
    {
        [Header("Switch Options")]

        // [SerializeField] private bool useVisuals = true;
        // [SerializeField] private GameObject switchVisual;
        // [SerializeField] Transform initialTra
        // [SerializeField] private bool animateVisuals = false;


        #region Actuator

        protected bool isOpen = true;
        public void OnTrigger()
        {
            isOpen = !isOpen;
            SignalManager.Instance.SendSignalFrom(isOpen, this);
        }
        public virtual void Focus() { }
        public virtual void Unfocus() { }

        #endregion
    }
}


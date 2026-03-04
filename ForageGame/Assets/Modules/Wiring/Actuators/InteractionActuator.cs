using Assets.Modules.Interaction;
using UnityEngine;

namespace Project.Signals.Actuators
{
    [RequireComponent(typeof(IActuator))]
    public class InteractionActuator : MonoBehaviour, IInteractable
    {
        private IActuator actuator;
        void Awake() => actuator = GetComponent<IActuator>();
        public void Interact() => actuator.OnTrigger();
        public void Focus() => actuator.Focus();
        public void Unfocus() => actuator.Unfocus();
    }
}

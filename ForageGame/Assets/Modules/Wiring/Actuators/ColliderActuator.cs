using Assets.Modules.Interaction;
using UnityEngine;

namespace Project.Signals.Actuators
{
    [RequireComponent(typeof(IActuator))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class ColliderActuator : MonoBehaviour
    {
        private IActuator actuator;
        void Awake() => actuator = GetComponent<IActuator>();
        public void OnTriggerEnter(Collider other) => actuator.OnTrigger();
    }
}

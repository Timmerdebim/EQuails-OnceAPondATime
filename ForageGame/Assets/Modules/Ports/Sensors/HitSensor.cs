using Assets.Modules.Interaction;
using UnityEngine;

namespace Project.Signals.Actuators
{
    [RequireComponent(typeof(IActuator))]
    [RequireComponent(typeof(Collider))]
    public class HitActuator : MonoBehaviour, IHitHandler
    {
        private IActuator actuator;
        void Awake() => actuator = GetComponent<IActuator>();
        public void Hit(float damage) => actuator.OnTrigger();
    }
}

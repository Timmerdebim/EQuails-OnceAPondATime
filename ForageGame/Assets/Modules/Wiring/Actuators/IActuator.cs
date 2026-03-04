using Assets.Modules.Interaction;
using UnityEngine;

namespace Project.Signals.Actuators
{
    public interface IActuator
    {
        public void OnTrigger();
        public void Focus();
        public void Unfocus();
    }
}

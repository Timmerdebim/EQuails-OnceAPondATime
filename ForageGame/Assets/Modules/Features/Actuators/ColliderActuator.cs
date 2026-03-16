using UnityEngine;
using UnityEngine.Events;

namespace TDK.Actuators
{
    [RequireComponent(typeof(Collider))]
    public class ColliderActuator : MonoBehaviour
    {
        public UnityEvent<GameObject> OnEmptyEntry;
        public UnityEvent<GameObject> OnEnterAny;
        public UnityEvent<GameObject> OnExitAny;
        public UnityEvent<GameObject> OnEmptyExit;

        private int _counter = 0;

        void OnTriggerEnter(Collider other)
        {
            _counter++;
            OnEnterAny.Invoke(other.gameObject);
            if (_counter == 1) OnEmptyEntry.Invoke(other.gameObject);
        }

        void OnTriggerExit(Collider other)
        {
            _counter--;
            OnExitAny.Invoke(other.gameObject);
            if (_counter == 0) OnEmptyExit.Invoke(other.gameObject);
        }
    }
}
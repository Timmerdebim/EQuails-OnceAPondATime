using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace TDK.Actuators
{
    [RequireComponent(typeof(Collider))]
    public class HitActuator : MonoBehaviour, IHitHandler
    {
        public UnityEvent<float> OnHit;

        public void Hit(float damage) => OnHit.Invoke(damage);
    }
}

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TDK.SaveSystem;
using UnityEngine;

namespace TDK.SaveSystem.Region
{
    [RequireComponent(typeof(Collider))]
    public abstract class RegionManager<T> : MonoBehaviour where T : class
    {
        [SerializeField] private Transform _fallbackGlobalRegion;
        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out T _))
                other.transform.parent = transform;
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out T _)) return;
            if (other.transform.parent != transform) return;

            other.transform.parent = _fallbackGlobalRegion;
        }

        public HashSet<T> GetInstances() => new(GetComponentsInChildren<T>(true));
    }
}
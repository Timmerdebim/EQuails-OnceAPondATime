using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Modules.Utils.Nearby
{
    public static class NearbyUtil<T> where T : Behaviour
    {
        public static List<T> GetNearbyObjects(Vector3 position, float radius, LayerMask layerMask)
        {
            List<T> nearbyObjects = new List<T>();

            RaycastHit[] hits = Physics.SphereCastAll(position, radius, Vector3.up, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.TryGetComponent<T>(out T obj) && obj.enabled)
                {
                    if (nearbyObjects.Contains(obj)) continue;
                    nearbyObjects.Add(obj);
                }
            }

            //Debug.Log(nearbyObjects.Count);
            return nearbyObjects;
        }
    }

    public class NearbyList<T> : IEnumerable<T> where T : Behaviour
    {
        private List<T> nearbyObjects = new List<T>();
        public List<T> NearbyObjects
        {
            get { return nearbyObjects; }
            private set { nearbyObjects = value; }
        }

        private CancellationTokenSource destructionTokenSource = new CancellationTokenSource();

        private float updateInterval = 0.1f; // seconds
        private Transform centerTransform;
        private float radius;
        private LayerMask layerMask;

        public NearbyList(float _updateInterval, Transform _centerTransform, float _radius, LayerMask _layerMask) 
        {
            updateInterval = _updateInterval;
            centerTransform = _centerTransform;
            radius = _radius;
            layerMask = _layerMask;

            Clock(destructionTokenSource.Token);
        }

        public void Stop()
        {
            destructionTokenSource.Cancel();
        }

        public async void Clock(CancellationToken ctx)
        {
            while (!ctx.IsCancellationRequested)
            {
                await Task.Delay((int)(updateInterval * 1000));
                NearbyUtil<T>.GetNearbyObjects(centerTransform.position, radius, layerMask);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return NearbyObjects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}

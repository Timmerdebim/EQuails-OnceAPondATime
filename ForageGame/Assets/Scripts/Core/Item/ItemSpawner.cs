using UnityEngine;

namespace TDK.ItemSystem
{
    public class SimpleItemSpawner : MonoBehaviour
    {
        [SerializeField] private ItemData _item;
        [SerializeField] private Vector3 _position;
        [SerializeField] private Transform _transform;

        public void Spawn() => ItemServices.Instance?.SpawnItem(_item, _position);

        public void SpawnLocal()
        {
            if (_transform == null) _transform = this.transform;
            ItemServices.Instance?.SpawnItem(_item, _position + _transform.position);
        }
    }
}
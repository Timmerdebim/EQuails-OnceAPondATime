using Project.Items;
using UnityEngine;

namespace TDK.ItemSystem
{
    public class SimpleItemSpawner : MonoBehaviour
    {
        [SerializeField] private Item _item;
        [SerializeField] private Vector3 _position;
        [SerializeField] private Transform _transform;

        public void Spawn() => ItemManager.Instance?.SpawnItemAt(_item, _position);

        public void SpawnLocal()
        {
            if (_transform == null) _transform = this.transform;
            ItemManager.Instance?.SpawnItemAt(_item, _position + _transform.position);
        }
    }
}
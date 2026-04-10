using UnityEngine;

namespace TDK.ItemSystem
{
    public class SimpleItemSpawner : MonoBehaviour
    {
        [SerializeField] private ItemData _item;
        [SerializeField] private Vector3 _position;
        [SerializeField] private Transform _transform;

        public void SpawnGlobal() => ItemServices.Instance?.SpawnItem(_item, _position);

        public void SpawnLocal()
        {
            ItemServices.Instance?.SpawnItem(_item, _position + this.transform.position);
        }

        public void SetItem(ItemData item)
        {
            _item = item;
        }
    }
}
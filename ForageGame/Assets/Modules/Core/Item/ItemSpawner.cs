using UnityEngine;
using FMODUnity;

namespace TDK.ItemSystem
{
    public class SimpleItemSpawner : MonoBehaviour
    {
        [SerializeField] private ItemData _item;
        [SerializeField] private Vector3 _position;
        [SerializeField] private EventReference spawnSfx; //leave null for non-sfx playing spawners, like the crafter

        public void SpawnGlobal()
        {
            ItemServices.Instance?.SpawnItem(_item, _position);
            if (spawnSfx.ToString().Length > 0) FMODUnity.RuntimeManager.PlayOneShot(spawnSfx, transform.position);
        }

        public void SpawnLocal()
        {
            ItemServices.Instance?.SpawnItem(_item, _position + transform.position);
            if (spawnSfx.ToString().Length > 0) FMODUnity.RuntimeManager.PlayOneShot(spawnSfx, transform.position);
        }

        public void SetItem(ItemData item)
        {
            _item = item;
        }

        public void SetAndSpawnItemLocal(ItemData item)
        {
            _item = item;
            SpawnLocal();
        }
    }
}
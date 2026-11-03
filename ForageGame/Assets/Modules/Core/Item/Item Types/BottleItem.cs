using UnityEngine;
using TDK.ItemSystem.Inventory;
using TDK.PlayerSystem;
using FMODUnity;

namespace TDK.ItemSystem.Types
{
    [CreateAssetMenu(fileName = "New Bottle", menuName = "Items/Bottle")]
    public class Bottle : ItemData
    {
        [SerializeField] protected ItemData waterBottle;
        [SerializeField] protected ItemData pollenBottle;
        [SerializeField] protected ItemData sporeBottle;
        [SerializeField] protected ItemData fireflyBottle;
        [SerializeField] protected EventReference fillSfx;

        public override bool TryWorldItemInteract()
        {
            if (!InventoryController.Instance.TryAddItemAtAny(this))
                return false;

            InventoryController.Instance.TryAddUnseenItem(this);
            return true;
        }

        public override bool TryUse()
        {
            Collider[] colliders = Physics.OverlapSphere(Player.Instance.transform.position, .5f);

            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out BottlePickupHandler handler))
                {
                    if (!InventoryController.Instance.TryRemoveItemAtCurrent(this))
                        return false;

                    if (!InventoryController.Instance.TryAddItemAtAny(handler.returnItem, false)) //bypass the sound effect for picking up the item
                        ItemServices.Instance?.SpawnItem(handler.returnItem, Player.Instance.transform.position);

                    RuntimeManager.PlayOneShot(fillSfx);
                    return true;
                }
            }
            return false;

        }

        public bool TryUseBottleToGetItem(ItemData item)
        {
            if (!InventoryController.Instance.TryRemoveItemAtCurrent(this))
                return false;

            if (!InventoryController.Instance.TryAddItemAtAny(item, false))
                ItemServices.Instance?.SpawnItem(item, Player.Instance.transform.position);
            return true;
        }
    }
}
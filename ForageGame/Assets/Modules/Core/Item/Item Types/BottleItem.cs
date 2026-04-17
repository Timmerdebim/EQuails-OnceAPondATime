using UnityEngine;
using TDK.ItemSystem.Inventory;
using TDK.PlayerSystem;

namespace TDK.ItemSystem.Types
{
    [CreateAssetMenu(fileName = "New Bottle", menuName = "Items/Bottle")]
    public class Bottle : ItemData
    {
        [SerializeField] protected ItemData waterBottle;
        [SerializeField] protected ItemData pollenBottle;
        [SerializeField] protected ItemData sporeBottle;
        [SerializeField] protected ItemData fireflyBottle;

        public override bool TryWorldItemInteract()
        {
            if (!InventoryController.Instance.TryAddItemAtAny(this))
                return false;

            InventoryController.Instance.TryAddUnseenItem(this);
            return true;
        }

        public override bool TryUse()
        {
            // // Get colliders and check if any of them have the correct tag
            // // if standing in water
            // {
            //     return TryUseBottleToGetItem(waterBottle);
            // }
            // // if standing in pollen cloud
            // {
            //     return TryUseBottleToGetItem(pollenBottle);
            // }
            // // if standing in spore cloud
            // {
            //     return TryUseBottleToGetItem(sporeBottle);
            // }
            // // if standing in firefly cloud
            // {
            //     return TryUseBottleToGetItem(fireflyBottle);
            // }

            return true;
        }

        private bool TryUseBottleToGetItem(ItemData item)
        {
            if (!InventoryController.Instance.TryRemoveItemAtCurrent(this))
                return false;

            if (!InventoryController.Instance.TryAddItemAtAny(item))
                ItemServices.Instance?.SpawnItem(item, Player.Instance.transform.position);
            return true;
        }
    }
}
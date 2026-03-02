using UnityEngine;
using Project.Items.Inventory;

namespace Project.Items.Types
{
    [CreateAssetMenu(fileName = "New Bottle", menuName = "Items/Bottle")]
    public class Bottle : Item
    {
        [SerializeField] protected Item waterBottle;
        [SerializeField] protected Item pollenBottle;
        [SerializeField] protected Item sporeBottle;
        [SerializeField] protected Item fireflyBottle;

        public override bool TryWorldItemInteract()
        {
            if (!Inventory.Inventory.Instance.hotbar.TryAddItemAtAny(this))
                return false;

            // TODO: first time pickup screen
            Inventory.Inventory.Instance.itemPickupUI.TriggerNewItemPopup(this);
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

        private bool TryUseBottleToGetItem(Item item)
        {
            if (!Inventory.Inventory.Instance.hotbar.TryRemoveItemAtCurrent(this))
                return false;

            if (!Inventory.Inventory.Instance.hotbar.TryAddItemAtAny(item))
                ItemManager.Instance.SpawnItemAt(item, Player.Instance.transform.position);
            return true;
        }
    }
}
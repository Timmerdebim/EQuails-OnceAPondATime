using UnityEngine;
using TDK.ItemSystem.Inventory;

namespace TDK.ItemSystem.Types
{
    [CreateAssetMenu(fileName = "New Resource", menuName = "Items/Resource")]
    public class ResourceItem : ItemData
    {
        public readonly bool doInstantUse = false;

        public override bool TryWorldItemInteract()
        {
            if (!InventoryController.Instance.TryAddItemAtAny(this))
                return false;

            if (!InventoryController.Instance.seenItems.Contains(this))
            {
                InventoryController.Instance.seenItems.Add(this);
                InventoryController.Instance.itemPickupUI.TriggerNewItemPopup(this);
            }
            return true;
        }

        public override bool TryUse() => true;
    }
}
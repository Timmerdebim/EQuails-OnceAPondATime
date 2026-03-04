using UnityEngine;
using Project.Items.Inventory;

namespace Project.Items.Types
{
    [CreateAssetMenu(fileName = "New Resource", menuName = "Items/Resource")]
    public class ResourceItem : Item
    {
        public readonly bool doInstantUse = false;

        public override bool TryWorldItemInteract()
        {
            if (!Inventory.Inventory.Instance.hotbar.TryAddItemAtAny(this))
                return false;

            if (!Inventory.Inventory.Instance.seenItems.Contains(this))
            {
                Inventory.Inventory.Instance.seenItems.Add(this);
                Inventory.Inventory.Instance.itemPickupUI.TriggerNewItemPopup(this);
            }
            return true;
        }

        public override bool TryUse() => true;
    }
}
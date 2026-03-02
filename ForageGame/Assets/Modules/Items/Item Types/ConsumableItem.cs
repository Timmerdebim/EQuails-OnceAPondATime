using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Project.Items.Types
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
    public class ConsumableItem : Item
    {
        [SerializeField] protected float consumableEnergy;
        [SerializeField] protected Item returnItem;

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
            if (!Inventory.Inventory.Instance.hotbar.TryRemoveItemAtCurrent(this))
                return false;

            Player.Instance.energy.TakeDamage(-consumableEnergy); // a little botched but ok...

            if (returnItem != null)
            {
                if (!Inventory.Inventory.Instance.hotbar.TryAddItemAtAny(returnItem))
                    ItemManager.Instance.SpawnItemAt(returnItem, Player.Instance.transform.position);
            }
            return true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TDK.ItemSystem.Inventory;
using UnityEngine;
using TDK.PlayerSystem;

namespace TDK.ItemSystem.Types
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
    public class ConsumableItem : ItemData
    {
        [SerializeField] protected float consumableEnergy;
        [SerializeField] protected ItemData returnItem;

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

        public override bool TryUse()
        {
            if (!InventoryController.Instance.TryRemoveItemAtCurrent(this))
                return false;

            Player.Instance.energy.TakeDamage(-consumableEnergy); // a little botched but ok...
            Player.Instance.Upgrade(PlayerUpgradeType.Sprint);

            if (returnItem != null)
            {
                if (!InventoryController.Instance.TryAddItemAtAny(returnItem))
                    ItemServices.Instance?.SpawnItem(returnItem, Player.Instance.transform.position);
            }
            return true;
        }
    }
}

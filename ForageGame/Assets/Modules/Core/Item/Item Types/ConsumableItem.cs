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

            InventoryController.Instance.TryAddUnseenItem(this);
            return true;
        }

        public override bool TryUse()
        {
            if (!InventoryController.Instance.TryRemoveItemAtCurrent(this))
                return false;

            if(consumableEnergy >= 0) Player.Instance.energy.AddEnergy(-consumableEnergy);
            else Player.Instance.energy.Hit(consumableEnergy); //plays the vfx/sfx, but is throttled by iframes (won't fix)

            if (returnItem != null)
            {
                if (!InventoryController.Instance.TryAddItemAtAny(returnItem, false))
                    ItemServices.Instance?.SpawnItem(returnItem, Player.Instance.transform.position);
            }
            Player.Instance.playerData.hasConsumedItem = true;
            return true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TDK.ItemSystem.Inventory;
using UnityEngine;
using TDK.PlayerSystem;
using FMODUnity;

namespace TDK.ItemSystem.Types
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
    public class ConsumableItem : ItemData
    {
        [SerializeField] protected float consumableEnergy;
        [SerializeField] protected ItemData returnItem;
        [SerializeField] private EventReference consumeSfx; //i.e. is this a drink or an eat

        public override bool TryWorldItemInteract()
        {
            if (!InventoryController.Instance.TryAddItemAtAny(this))
                return false;

            InventoryController.Instance.TryAddUnseenItem(this);
            return true;
        }

        public bool TryUseOther() //i.e. Princess 'using' the bottle of water; skip all the player focused stuff, like Sfx
        {
            if (!InventoryController.Instance.TryRemoveItemAtCurrent(this))
                return false;

            if (returnItem != null)
            {
                if (!InventoryController.Instance.TryAddItemAtAny(returnItem, false))
                    ItemServices.Instance?.SpawnItem(returnItem, Player.Instance.transform.position);
            }
            return true;
        }

        public override bool TryUse()
        {
            if (!InventoryController.Instance.TryRemoveItemAtCurrent(this))
                return false;

            RuntimeManager.PlayOneShot(consumeSfx);
            if(consumableEnergy >= 0) Player.Instance.energy.TakeDamage(-consumableEnergy); //now I see why you did it like this, 10/10 code
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

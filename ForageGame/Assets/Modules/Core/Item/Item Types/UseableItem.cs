using UnityEngine;
using TDK.PlayerSystem;
using TDK.ItemSystem.Inventory;

namespace TDK.ItemSystem.Types
{
    [CreateAssetMenu(fileName = "New Useable", menuName = "Items/Useable")]
    public class UseableItem : ItemData
    {
        [SerializeField] protected PlayerUpgradeType upgradeType = PlayerUpgradeType.Attack;

        public override bool TryWorldItemInteract()
        {
            if (!Player.Instance) return false;
            Player.Instance?.Upgrade(upgradeType);
            //InventoryController.Instance?.itemPickupUI.TriggerNewItemPopup(this);
            InventoryController.Instance.TryAddUnseenItem(this); //I need the item to be added to 'seenItems' so the Npc's can react to it. I don't see an issue with doing it this way instead? ~Lars
            return true;
        }

        public override bool TryUse() => false;
    }
}

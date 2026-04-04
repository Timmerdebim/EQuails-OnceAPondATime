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
            InventoryController.Instance?.itemPickupUI.TriggerNewItemPopup(this);
            return true;
        }

        public override bool TryUse() => false;
    }
}

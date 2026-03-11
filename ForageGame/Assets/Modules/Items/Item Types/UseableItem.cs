using UnityEngine;

namespace Project.Items.Types
{
    [CreateAssetMenu(fileName = "New Useable", menuName = "Items/Useable")]
    public class UseableItem : Item
    {
        [SerializeField] protected PlayerUpgradeType upgradeType = PlayerUpgradeType.Attack;

        public override bool TryWorldItemInteract()
        {
            if (!Player.Instance) return false;
            Player.Instance?.Upgrade(upgradeType);
            Inventory.Inventory.Instance.itemPickupUI.TriggerNewItemPopup(this);
            return true;
        }

        public override bool TryUse() => false;
    }
}

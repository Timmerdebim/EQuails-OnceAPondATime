using UnityEngine;
using System.Collections.Generic;

namespace TDK.ItemSystem.Inventory
{
    public abstract class ItemContainer : MonoBehaviour
    {
        public List<ItemSlot> Slots { get; protected set; } = new();
        protected int currentSlotIndex = 0;

        #region Add Functions

        public bool TryAddItemAtIndex(ItemData item, int slotIndex, int quantity = 1)
        {
            if (!IsSlotValid(slotIndex))
                return false;

            return Slots[slotIndex].TryAddItem(item, quantity);
        }

        public bool TryAddItemAtCurrent(ItemData item, int quantity = 1) => TryAddItemAtIndex(item, currentSlotIndex, quantity);

        public bool TryAddItemAtAny(ItemData item, int quantity = 1)
        {
            // First try to stack with existing items
            foreach (ItemSlot slot in Slots)
            {
                if (slot.ContainsItem(item))
                {
                    if (slot.TryAddItem(item, quantity))
                        return true;
                }
            }
            // Then try to find an empty slot
            foreach (ItemSlot slot in Slots)
            {
                if (slot.IsEmpty())
                {
                    if (slot.TryAddItem(item, quantity))
                        return true;
                }
            }
            return false;
        }

        #endregion

        #region Remove Functions

        public bool TryRemoveItemAtIndex(ItemData item, int slotIndex, int quantity = 1)
        {
            if (!IsSlotValid(slotIndex))
                return false;

            return Slots[slotIndex].TryRemoveItem(item, quantity);
        }

        public bool TryRemoveItemAtCurrent(ItemData item, int quantity = 1) => TryRemoveItemAtIndex(item, currentSlotIndex, quantity);

        public bool TryRemoveAnyAtIndex(int slotIndex, int quantity = 1)
        {
            if (!IsSlotValid(slotIndex))
                return false;

            return TryRemoveItemAtIndex(Slots[slotIndex].Item, slotIndex, quantity);
        }

        public bool TryRemoveAnyAtCurrent(int quantity = 1) => TryRemoveAnyAtIndex(currentSlotIndex, quantity);

        #endregion

        public bool IsSlotValid(int index) => index >= 0 && index < Slots.Count;

        #region Slot Selection

        public void SelectSlot(int index)
        {
            if (IsSlotValid(index))
            {
                if (IsSlotValid(currentSlotIndex))
                    Slots[currentSlotIndex].SetSelected(false);
                currentSlotIndex = index;
                Slots[currentSlotIndex].SetSelected(true);
            }
        }

        public void SelectNext() => SelectSlot(currentSlotIndex + 1);
        public void SelectPrevious() => SelectSlot(currentSlotIndex - 1);

        public ItemData GetItemAtIndex(int index)
        {
            if (IsSlotValid(index))
                return Slots[index].Item;
            else return null;
        }

        public ItemData GetItemAtCurrent() => GetItemAtIndex(currentSlotIndex);

        #endregion
    }
}
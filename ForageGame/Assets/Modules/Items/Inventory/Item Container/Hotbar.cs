using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Project.Items.Inventory
{
    public class Hotbar : ItemContainer
    {
        [SerializeField] private int initialSlotCount = 3;
        [SerializeField] private GameObject slotPrefab;

        private void Awake()
        {
            InitializeSlots(initialSlotCount);
            currentSlotIndex = 0;
            SelectSlot(currentSlotIndex);
        }

        public void InitializeSlots(int slotCount)
        {
            Slots.Clear();
            foreach (Transform child in transform) Destroy(child.gameObject);
            AddSlots(slotCount);
        }

        public void AddSlots(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject slotObject = Instantiate(slotPrefab, transform);
                InventorySlot slot = slotObject.GetComponent<InventorySlot>();
                slot.Initialize();
                Slots.Add(slot);
            }
        }

        #region Triggers

        public bool TryUseItem()
        {
            if (!IsSlotValid(currentSlotIndex))
                return false;
            if (Slots[currentSlotIndex].IsEmpty())
                return false;
            return Slots[currentSlotIndex].Item.TryUse();
        }

        public bool TryDropItem()
        {
            if (!IsSlotValid(currentSlotIndex))
                return false;
            Item item = Slots[currentSlotIndex].Item; // do this because the item will be removed in the next step

            if (!TryRemoveAnyAtCurrent())
                return false;

            ItemManager.Instance.SpawnItemAt(item, Player.Instance.transform.position);
            return true;
        }

        // public bool GiveItem(Item item)
        // {
        //     for (int i = 0; i < hotbarSize; i++)
        //     {
        //         if (item == hotbarItems[i])
        //         {
        //             RemoveItem(i);
        //             return true;
        //         }
        //     }
        //     return false;
        // }

        #endregion

        #region Save & Load

        public void SetData(List<InventorySlotData> data)
        {
            InitializeSlots(data.Count);

            for (int i = 0; i < data.Count; i++)
                Slots[i].SetData(data[i]);
        }

        public List<InventorySlotData> GetData()
        {
            List<InventorySlotData> hotbarData = new();

            foreach (var slot in Slots)
                hotbarData.Add(slot.GetData());

            return hotbarData;
        }

        #endregion
    }
}
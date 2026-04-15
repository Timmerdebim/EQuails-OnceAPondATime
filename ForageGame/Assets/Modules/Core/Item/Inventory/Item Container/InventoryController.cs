using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TDK.SaveSystem;
using TDK.PlayerSystem;
using System;

namespace TDK.ItemSystem.Inventory
{
    public class InventoryController : ItemContainer, ILoadable
    {
        public static InventoryController Instance;
        [SerializeField] private int initialSlotCount = 3;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] public ItemPickupUI itemPickupUI;

        public static event Action<ItemData> onNewItemSeen;

        public HashSet<ItemData> seenItems = new();


        public void TryAddUnseenItem(ItemData item)
        {
            if (!seenItems.Contains(item))
            {
                seenItems.Add(item);
                itemPickupUI.TriggerNewItemPopup(item);
                onNewItemSeen?.Invoke(item);
            }
        }


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            Initialize(initialSlotCount);
        }

        public void Initialize(int slotCount)
        {
            Slots.Clear();
            foreach (Transform child in transform) Destroy(child.gameObject);
            AddSlots(slotCount);
            currentSlotIndex = 0;
            SelectSlot(currentSlotIndex);
        }
        public void Initialize(List<ItemSlotSaveData> data)
        {
            Slots.Clear();
            foreach (Transform child in transform) Destroy(child.gameObject);

            foreach (ItemSlotSaveData dataEntry in data)
            {
                ItemSlot slot = AddSlot();
                slot.Initialize(dataEntry);
            }
            currentSlotIndex = 0;
            SelectSlot(currentSlotIndex);
        }


        public ItemSlot AddSlot()
        {
            GameObject slotObject = Instantiate(slotPrefab, transform);
            ItemSlot slot = slotObject.GetComponent<ItemSlot>();
            slot.Initialize();
            Slots.Add(slot);
            return slot;
        }
        public void AddSlots(int count)
        {
            for (int i = 0; i < count; i++)
                AddSlot();
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
            ItemData item = Slots[currentSlotIndex].Item; // do this because the item will be removed in the next step

            if (!TryRemoveAnyAtCurrent())
                return false;

            ItemServices.Instance.SpawnItem(item, Player.Instance.transform.position);
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

        public void LoadData(WorldSaveData data)
        {
            Initialize(data.Inventory.Items);
        }

        #endregion
    }
}
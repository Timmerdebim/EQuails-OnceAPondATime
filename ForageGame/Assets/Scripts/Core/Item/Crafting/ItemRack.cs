using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

// IMPORTANT: ItemRacks cannot overlap; this will result in breaking possibly everything!
namespace TDK.ItemSystem.Inventory
{
    public enum ItemRackAlignment { Left, Right, Center, Justified }

    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(SplineContainer))]
    public class ItemRack : MonoBehaviour
    {
        [SerializeField] private ItemRackAlignment alignment = ItemRackAlignment.Left;
        [SerializeField] private float suckDuration = 1f;

        [SerializeField] private List<ItemController> _itemControllers = new();
        private SplineContainer splineContainer;

        void Awake()
        {
            splineContainer = GetComponent<SplineContainer>();
        }

        void Update()
        {
            if (_itemControllers.Contains(null)) // i need to think of a better solution to this (on trigger exit wont work since it is not triggered by Destroy)
                RefreshItems();
        }

        void OnTriggerEnter(Collider other)
        {
            ItemController controller = other.gameObject.GetComponent<ItemController>();
            if (controller)
                AddItem(controller);
        }

        public void RefreshItems()
        {
            _itemControllers.RemoveAll(item => item == null);
            UpdateItemPositions();
        }

        #region Set Spline Position

        public void UpdateItemPositions()
        {
            float dt = 1f / Mathf.Max(1, _itemControllers.Count);

            for (int i = 0; i < _itemControllers.Count; i++)
            {
                Vector3 target = Vector3.zero;
                switch (alignment)
                {
                    case ItemRackAlignment.Left:
                        target = splineContainer.EvaluatePosition(dt * i);
                        break;
                    case ItemRackAlignment.Right:
                        target = splineContainer.EvaluatePosition(dt * (i + 1));
                        break;
                    case ItemRackAlignment.Center:
                        target = splineContainer.EvaluatePosition(dt * i + dt / 2);
                        break;
                    case ItemRackAlignment.Justified:
                        target = splineContainer.EvaluatePosition(1 / (1 / dt - 1) * i);
                        break;
                }
                _itemControllers[i]?.MoveTo(target, suckDuration);
            }
        }

        #endregion

        #region Getting & Setting

        public List<ItemController> GetItemControllers() => new(_itemControllers);  // RETURNS A COPY

        public List<ItemData> GetItems() // RETURNS A COPY
        {
            List<ItemData> items = new();
            foreach (ItemController controller in GetItemControllers())
                items?.Add(controller.ItemData);
            return items;
        }

        public bool ContainsItem(ItemData item) => GetItems().Contains(item);

        public bool ContainsItems(List<ItemData> items)
        {
            List<ItemData> itemsCopy = GetItems();
            foreach (ItemData item in items)
            {
                if (!itemsCopy.Remove(item))
                    return false;
            }
            return true;
        }

        public bool ContainsItemsExactly(List<ItemData> items)
        {
            if (items.Count != GetItems().Count)
                return false;
            return ContainsItems(items);
        }

        public void AddItem(ItemController controller)
        {
            if (_itemControllers.Contains(controller))
                return;
            _itemControllers.Add(controller);
            RefreshItems();
        }

        public void RemoveItem(ItemController controller)
        {
            if (!_itemControllers.Contains(controller))
                return;
            _itemControllers.Remove(controller);
            RefreshItems();
        }

        public void RemoveItem(ItemData data)
        {
            foreach (ItemController controller in _itemControllers)
            {
                if (controller.ItemData == data)
                {
                    RemoveItem(controller);
                    return;
                }
            }
        }

        public void RemoveItems(List<ItemData> items)
        {
            foreach (ItemData item in items)
                RemoveItem(item);
        }

        public void RemoveAll()
        {
            _itemControllers = new();
            RefreshItems();
        }

        #endregion
    }
}

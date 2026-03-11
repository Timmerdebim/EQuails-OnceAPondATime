using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Project.Items
{
    public class ItemManager : MonoBehaviour
    {
        public static ItemManager Instance { get; private set; }
        [SerializeField] private GameObject worldItemPrefab;
        [SerializeField] private Item[] itemDatabase;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public int GetIdByItem(Item item)
        {
            return Array.FindIndex(itemDatabase, row => row == item); // Returns -1 if not in database
        }

        public Item GetItemById(int id)
        {
            if (id < 0 || id >= itemDatabase.Count())
                return null;
            return itemDatabase[id];
        }

        public void SpawnItemAt(Item item, Vector3 position)
        {
            GameObject worldItem = Instantiate(worldItemPrefab, position, Quaternion.identity);
            worldItem.GetComponent<WorldItem>().Initialize(item);

            worldItem.transform.localScale = Vector3.zero;
            worldItem.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InOutBack);
        }
    }
}
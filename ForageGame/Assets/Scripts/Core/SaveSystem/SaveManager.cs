using Project.Items.Inventory;
using UnityEngine;

namespace TDK.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        private SaveData _currentSaveData;
        private int _currentSaveSlot = -1;
        public int CurrentSaveSlot
        {
            get => _currentSaveSlot;
            set
            {
                _currentSaveSlot = value;
                PlayerPrefs.SetInt("lastSlotIndexUsed", value);
                PlayerPrefs.Save();
            }
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        // ------------ Load ------------

        public void LoadGame()
        {
            _currentSaveData = SaveServices.GetSaveFile(_currentSaveSlot);

            Player.Instance?.LoadData(_currentSaveData.playerData);
            Inventory.Instance?.LoadData(_currentSaveData.inventoryData);
        }

        public void LoadRegion()
        {

        }

        // ------------ Save ------------

        public void SaveGame()
        {
            if (_currentSaveData == null || _currentSaveSlot < 0)
                return;

            Player.Instance?.SaveData(ref _currentSaveData.playerData);
            Inventory.Instance?.SaveData(ref _currentSaveData.inventoryData);

            SaveServices.SetSaveFile(_currentSaveSlot, _currentSaveData);
        }

        public void SaveRegion()
        {

        }

        // TODO: add autosaving
    }
}
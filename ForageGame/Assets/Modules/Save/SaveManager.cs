using UnityEngine;

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
        _currentSaveData = SaveSystem.GetSaveFile(_currentSaveSlot);

        Player.Instance?.LoadData(_currentSaveData.playerData);
        // TODO: storyflags, inventory
    }

    public void LoadRegion()
    {

    }

    // ------------ Save ------------

    public void SaveGame()
    {
        if (_currentSaveData == null || _currentSaveSlot < 0)
            return;

        Player.Instance?.LoadData(_currentSaveData.playerData);

        SaveSystem.SetSaveFile(_currentSaveSlot, _currentSaveData);
        // TODO: storyflags, inventory
    }

    public void SaveRegion()
    {

    }

    // TODO: add autosaving
}

using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public string characterName;
    public float playtimeSeconds;
    public int level;
    public Vector3 playerPosition;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private SaveData currentSaveData;
    private int currentSaveSlot = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NewGame(int slotIndex)
    {
        currentSaveSlot = slotIndex;
        currentSaveData = new SaveData
        {
            characterName = "Player",
            playtimeSeconds = 0,
            level = 1,
            playerPosition = Vector3.zero
        };

        SaveGame();
        // Load game scene
    }

    public void LoadGame(int slotIndex)
    {
        currentSaveSlot = slotIndex;
        currentSaveData = LoadSaveData(slotIndex);
        // Load game scene with saved data
    }

    public SaveData LoadSaveData(int slotIndex)
    {
        string path = Path.Combine("Assets/SaveData", $"save_{slotIndex}.dat");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }

        return null;
    }

    public void SaveGame()
    {
        if (currentSaveSlot < 0 || currentSaveData == null)
            return;

        string directory = "Assets/SaveData";
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        string path = Path.Combine(directory, $"save_{currentSaveSlot}.dat");
        string json = JsonUtility.ToJson(currentSaveData, true);
        File.WriteAllText(path, json);
    }
}
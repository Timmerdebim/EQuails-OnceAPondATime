using System.IO;
using UnityEngine;

public static class SaveSystem
{
    // static string path = Application.persistentDataPath + "/SaveData";
    static string path = "Assets/SaveData";

    public static bool SaveFileExists(int slotIndex)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return File.Exists(path + "/SaveSlot" + slotIndex + ".json");
    }

    public static SaveData GetSaveFile(int slotIndex)
    {
        if (!SaveFileExists(slotIndex))
            CreateSaveFile(slotIndex);

        string filePath = path + "/SaveSlot" + slotIndex + ".json";
        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void SetSaveFile(int slotIndex, SaveData saveData)
    {
        if (!SaveFileExists(slotIndex))
            CreateSaveFile(slotIndex);

        string filePath = path + "/SaveSlot" + slotIndex + ".json";
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(filePath, json);
        // TODO: STORY FLAGS

        Debug.Log($"SAVE: Wrote to save file {slotIndex}.");
    }

    public static void CreateSaveFile(int slotIndex)
    {
        if (SaveFileExists(slotIndex))
        {
            Debug.Log("SAVE: ERROR: Cannot make new save file, old save file exists.");
            return;
        }

        SaveData saveData = new SaveData();

        string filePath = path + "/SaveSlot" + slotIndex + ".json";
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(filePath, json);

        Debug.Log($"SAVE: Created save file {slotIndex}.");
    }

    public static void DeleteSaveFile(int slotIndex)
    {
        if (!SaveFileExists(slotIndex))
        {
            Debug.Log($"SAVE: ERROR: Save file {slotIndex} does not exist.");
            return;
        }

        string filePath = path + "/SaveSlot" + slotIndex + ".json";
        File.Delete(filePath);

        Debug.Log($"SAVE: Deleted save file {slotIndex}.");
    }
}

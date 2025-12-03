using System.IO;
using UnityEngine;

public static class SaveSystem
{
    static string path = Application.persistentDataPath + "/slot";

    public static bool SaveFileExists(int slot)
    {
        return File.Exists(path + slot + ".json");
    }

    public static void NewSaveFile(int slot)
    {
        if (SaveFileExists(slot))
        {
            Debug.Log("SAVE: ERROR: Cannot make new save file, old save file exists.");
            return;
        }

        // TODO

        Debug.Log("SAVE: Save file " + slot + " made.");
    }

    public static void LoadSaveFile(int slot)
    {
        if (!SaveFileExists(slot))
        {
            Debug.Log("SAVE: ERROR: Cannot load save file, save file does not exist.");
            return;
        }

        // TODO

        Debug.Log("SAVE: Save file " + slot + " loaded.");
    }

    public static void SaveSaveFile(int slot)
    {
        if (!SaveFileExists(slot))
        {
            Debug.Log("SAVE: ERROR: Cannot save save file, save file does not exist.");
            return;
        }

        // TODO

        Debug.Log("SAVE: Save file " + slot + " saved.");
    }

    public static void DeleteSaveFile(int slot)
    {
        if (!SaveFileExists(slot))
        {
            Debug.Log("SAVE: ERROR: Cannot delete save file, save file does not exist.");
            return;
        }

        string file = path + slot + ".json";
        File.Delete(file);

        Debug.Log("SAVE: Save file " + slot + " deleted.");
    }
}

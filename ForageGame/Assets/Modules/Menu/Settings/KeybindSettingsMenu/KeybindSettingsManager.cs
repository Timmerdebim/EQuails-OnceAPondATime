using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class KeybindSettingsManager : MonoBehaviour
{
    public static KeybindSettingsManager Instance { get; private set; }
    [SerializeField] private string settingsPath = "Assets/Settings/keybinds.json";
    [SerializeField] private InputActionAsset actions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    public void LoadSettings()
    {
        //string rebinds = PlayerPrefs.GetString("rebinds");
        //if (!string.IsNullOrEmpty(rebinds))
        //    actions.LoadBindingOverridesFromJson(rebinds);

        if (File.Exists(settingsPath))
        {
            string settingsJson = File.ReadAllText(settingsPath);
            if (!string.IsNullOrEmpty(settingsJson))
                actions.LoadBindingOverridesFromJson(settingsJson);
        }
    }

    public void SaveSettings()
    {
        //string rebinds = actions.SaveBindingOverridesAsJson();
        //PlayerPrefs.SetString("rebinds", rebinds);

        if (!Directory.Exists(settingsPath))
            Directory.CreateDirectory(settingsPath);

        string settingsJson = actions.SaveBindingOverridesAsJson();

        File.WriteAllText(settingsPath, settingsJson);
    }
}

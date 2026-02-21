using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class KeybindSettingsManager : MonoBehaviour
{
    public static KeybindSettingsManager Instance { get; private set; }
    [SerializeField] private string settingsPath = "Assets/Settings";
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

        string keybindPath = Path.Combine(settingsPath, "keybind.json");

        if (File.Exists(keybindPath))
        {
            string settingsJson = File.ReadAllText(keybindPath);
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

        File.WriteAllText(Path.Combine(settingsPath, "keybind.json"), settingsJson);
    }
}

using Project.Menus.Keybind;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Prompt", menuName = "Prompts/Prompt")]
public class HelpPrompt : ScriptableObject
{
    [SerializeField] private string promptName;
    [SerializeField] private string actionName;

    public string GetName() => promptName;
    public string GetAction() => actionName;
}

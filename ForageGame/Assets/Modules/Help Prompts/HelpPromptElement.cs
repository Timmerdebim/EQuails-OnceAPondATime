using Project.Menus.Keybind;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HelpPromptElement : MonoBehaviour
{
    public string promptName { get; private set; }
    private InputAction action;
    private HelpPrompts parent;
    [SerializeField] public TMP_Text label;
    [SerializeField] public Image image;

    public void Initialize(HelpPrompts parent, string promptName, InputAction action)
    {
        this.promptName = promptName;
        this.parent = parent;
        label.text = promptName;
        this.action = action;
        RefreshVisuals();
    }

    public void RefreshVisuals()
    {
        var displayString = string.Empty;
        var deviceLayoutName = default(string);
        var controlPath = default(string);
        int bindingIndex = -1;

        if (action != null)
        {
            foreach (var binding in action.bindings)
            {
                var control = InputControlPath.TryFindControl(parent.lastUsedInputDevice, binding.effectivePath);
                if (control != null) bindingIndex = action.GetBindingIndexForControl(control);
            }
            if (bindingIndex < 0) return;

            displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath);
            Debug.Log(deviceLayoutName);
            Debug.Log(controlPath);
        }

        image.sprite = KeybindSpritesDatabase.Instance?.GetKeybindSprite(displayString, deviceLayoutName, controlPath);
    }


}

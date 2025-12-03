using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using TMPro;

public class KeyRebindUI : MonoBehaviour
{
    public InputActionReference actionReference; // The action to rebind
    public int bindingIndex = 0;                 // Which binding (e.g., for composites)
    public TextMeshProUGUI bindingDisplayNameText;
    public Button rebindButton;

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    public void Refresh()
    {
        UpdateUI();
        rebindButton.onClick.AddListener(StartRebinding);
    }

    public void UpdateUI()
    {
        var binding = actionReference.action.bindings[bindingIndex];
        bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
            binding.effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    public void StartRebinding()
    {
        bindingDisplayNameText.text = "Press any key...";

        rebindOperation = actionReference.action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f) // avoids catching modifier-only presses
            .OnComplete(operation =>
            {
                operation.Dispose();
                SaveRebinds();
                UpdateUI();
            })
            .Start();
    }

    private void SaveRebinds()
    {
        string json = actionReference.action.actionMap.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", json);
        PlayerPrefs.Save();
    }
}

using System;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthUI : MonoBehaviour
{
    private ProgressBar energybar { get; set; }
    private int _clickCount;

    //Add logic that interacts with the UI controls in the `OnEnable` methods
    private void OnEnable()
    {
        // The UXML is already instantiated by the UIDocument component
        var uiDocument = GetComponent<UIDocument>();

        energybar = uiDocument.rootVisualElement.Q("energybar") as ProgressBar;

        // _button.RegisterCallback<ClickEvent>(PrintClickMessage);
        // var _inputFields = uiDocument.rootVisualElement.Q("input-message");
        // _inputFields.RegisterCallback<ChangeEvent<string>>(InputMessage);
    }

    public void SetEnergy(float energy)
    {
        energybar.value = energy;
    }

    // private void OnDisable()
    // {
    //     _button.UnregisterCallback<ClickEvent>(PrintClickMessage);
    // }

    // private void PrintClickMessage(ClickEvent evt)
    // {
    //     ++_clickCount;
    //
    //     Debug.Log($"{"button"} was clicked!" +
    //               (_toggle.value ? " Count: " + _clickCount : ""));
    // }
    //
    // public static void InputMessage(ChangeEvent<string> evt)
    // {
    //     Debug.Log($"{evt.newValue} -> {evt.target}");
    // }
}

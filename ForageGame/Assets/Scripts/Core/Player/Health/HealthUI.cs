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
    }

    public void SetEnergy(float energy)
    {
        energybar.value = energy;
    }
}

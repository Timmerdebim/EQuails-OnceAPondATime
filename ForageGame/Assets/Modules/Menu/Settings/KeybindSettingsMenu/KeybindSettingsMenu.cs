using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class KeybindSettingsMenu : Menu
{
    [Header("UI References")]
    [SerializeField] private Button resetButton;

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    // ------------ Buttons ------------

    public void OnResetButtonClicked()
    {
        //foreach (var actionMap in inputActionAsset.actionMaps)
        //    actionMap.RemoveAllBindingOverrides();
        //
        //RefreshMenu();
    }

    // ------------ Functions ------------
}
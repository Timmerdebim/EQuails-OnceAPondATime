using System.Linq;
using Project.Menus.Keybind;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.U2D;
using System;

public static class KeybindSystem
{
    private static InputDevice lastDevice;
    private static IDisposable subscription;
 
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        lastDevice = null;
 
        subscription?.Dispose();
        subscription = InputSystem.onAnyButtonPress.Call(control => lastDevice = control.device);
    }

    public static InputControl GetLastUsedInputControl()
    {
        if (lastDevice is Gamepad gamepad && gamepad.added)
            return gamepad;
 
        return Keyboard.current;
    }

    public static Sprite GetLastUsedKeybindVisual(InputAction action)
    {
        InputControl inputControl = GetLastUsedInputControl();

        if (action == null) return null;

        foreach (var binding in action.bindings)
        {
            var control = InputControlPath.TryFindControl(inputControl, binding.effectivePath);
            if (control != null)
            {
                int bindingIndex = action.GetBindingIndexForControl(control);
                if (bindingIndex >= 0)
                {
                    string displayString = action.GetBindingDisplayString(bindingIndex, out string deviceLayoutName, out string controlPath);
                    return KeybindSpritesDatabase.Instance?.GetKeybindSprite(displayString, deviceLayoutName, controlPath);
                }
            }
        }
        return null;
    }
}

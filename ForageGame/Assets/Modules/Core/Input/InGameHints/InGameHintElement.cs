using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameHintElement : MonoBehaviour
{
    [SerializeField] private InputActionReference inputActionReference;
    [SerializeField] private Image image;

    public void RefreshVisuals()
    {
        image.sprite = KeybindSystem.GetKeybindVisual(inputActionReference.action, KeybindSystem.DeviceType.LastUsed);
    }
}

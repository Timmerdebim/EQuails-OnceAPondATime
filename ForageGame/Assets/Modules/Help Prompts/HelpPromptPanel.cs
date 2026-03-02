using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using Project.Items.Inventory;
using System;
using Project.Items.Types;
using UnityEngine.UI;
using TMPro;
using Project.Menus.Keybind;

[RequireComponent(typeof(CanvasGroup))]
public class HelpPromptPanel : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private GameObject promptPrefab;
    private HashSet<HelpPrompt> currentPrompts = new();
    private enum HelpPromptPanelState { Activated, Deactivating, Deactivated, Activating }
    private HelpPromptPanelState state = HelpPromptPanelState.Activated;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        HidePrompts();
    }

    private float idleTime = 0;
    private readonly float activationTime = 5;

    void Update()
    {
        if (Input.anyKeyDown)
            HidePrompts();
        else if (idleTime < activationTime)
            idleTime += Time.deltaTime;
        else
            ShowPrompts();
    }

    #region Show & Hide Current Prompts

    private Tween tween;
    private CanvasGroup canvasGroup;

    private void ShowPrompts()
    {
        if (state == HelpPromptPanelState.Activated || state == HelpPromptPanelState.Activating)
            return;
        state = HelpPromptPanelState.Activating;
        RefreshPrompts();
        InputDevice lastUsedInputDevice = InputSystem.devices
            .Where(d => d.lastUpdateTime > 0)
            .OrderByDescending(d => d.lastUpdateTime)
            .FirstOrDefault();
        foreach (HelpPrompt prompt in currentPrompts)
            CreatePrompt(prompt, lastUsedInputDevice);

        tween?.Kill();
        tween = canvasGroup.DOFade(1, 1).SetEase(Ease.InOutQuad)
        .OnComplete(() => state = HelpPromptPanelState.Activated);
    }

    private void HidePrompts()
    {
        idleTime = 0;
        if (state == HelpPromptPanelState.Deactivated || state == HelpPromptPanelState.Deactivating)
            return;
        state = HelpPromptPanelState.Deactivating;
        tween?.Kill();
        tween = canvasGroup.DOFade(0, 0.2f).SetEase(Ease.InOutQuad)
        .OnComplete(() =>
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
            state = HelpPromptPanelState.Deactivated;
        });
    }

    public void CreatePrompt(HelpPrompt prompt, InputDevice inputDevice)
    {
        GameObject promptObject = Instantiate(promptPrefab, transform);
        Image image = promptObject.GetComponentInChildren<Image>();
        TMP_Text label = promptObject.GetComponentInChildren<TMP_Text>();
        label.text = prompt.GetName();

        InputAction action = inputActions.FindAction(prompt.GetAction());

        if (action != null)
        {
            foreach (var binding in action.bindings)
            {
                var control = InputControlPath.TryFindControl(inputDevice, binding.effectivePath);
                if (control != null)
                {
                    int bindingIndex = action.GetBindingIndexForControl(control);
                    if (bindingIndex >= 0)
                    {
                        string displayString = action.GetBindingDisplayString(bindingIndex, out string deviceLayoutName, out string controlPath);
                        image.sprite = KeybindSpritesDatabase.Instance?.GetKeybindSprite(displayString, deviceLayoutName, controlPath);
                        return;
                    }
                }
            }
        }
    }

    #endregion


    #region Refresh Prompts

    [Header("Built In Help Prompts")]
    [SerializeField] private HelpPrompt consumePrompt;
    [SerializeField] private HelpPrompt interactPrompt;


    public void RefreshPrompts()
    {
        if (Inventory.Instance.hotbar.GetItemAtCurrent() is ConsumableItem)
            currentPrompts.Add(consumePrompt);
        else currentPrompts.Remove(consumePrompt);

        if (Player.Instance.playerInteract.GetCurrentFocus() != null)
            currentPrompts.Add(interactPrompt);
        else currentPrompts.Remove(interactPrompt);
    }

    #endregion
}

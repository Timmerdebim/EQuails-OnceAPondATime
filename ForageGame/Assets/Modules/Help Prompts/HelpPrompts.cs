using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

[RequireComponent(typeof(CanvasGroup))]
public class HelpPrompts : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    public InputDevice lastUsedInputDevice { get; private set; }
    [SerializeField] private GameObject promptPrefab;
    private HashSet<GameObject> promptsToAdd = new();
    private HashSet<GameObject> promptsToRemove = new();
    private CanvasGroup canvasGroup;
    private Tween tween;
    private float idleTime = 0;
    private readonly float activationTime = 10;
    private enum HelpPromptState { Activated, Deactivating, Deactivated, Activating }
    private HelpPromptState state = HelpPromptState.Deactivated;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        RefreshVisuals();
        HidePrompts();
        AddPrompt("Consume", "Use Item");
        AddPrompt("Interact", "Interact");
    }

    void Update()
    {
        RefreshVisuals();

        if (Input.anyKeyDown)
            HidePrompts();
        else if (idleTime < activationTime)
            idleTime += Time.deltaTime;
        else
            ShowPrompts();
    }

    #region Show & Hide Current Prompts

    private void ShowPrompts()
    {
        if (state == HelpPromptState.Activated || state == HelpPromptState.Activating)
            return;
        state = HelpPromptState.Activating;
        tween?.Kill();
        tween = canvasGroup.DOFade(1, 1).SetEase(Ease.InOutQuad)
        .OnComplete(() => state = HelpPromptState.Activated);
    }

    private void HidePrompts()
    {
        idleTime = 0;
        if (state == HelpPromptState.Deactivated || state == HelpPromptState.Deactivating)
            return;
        state = HelpPromptState.Deactivating;
        tween?.Kill();
        tween = canvasGroup.DOFade(0, 0.2f).SetEase(Ease.InOutQuad)
        .OnComplete(() =>
        {
            state = HelpPromptState.Deactivated;
            ModifyCurrentPrompts();
        });
    }

    #endregion

    #region Modify Current Prompts

    public void AddPrompt(string promptName, string promptAction)
    {
        GameObject promptObject = Instantiate(promptPrefab, transform);
        HelpPromptElement prompt = promptObject.GetComponent<HelpPromptElement>();
        prompt.Initialize(this, promptName, inputActions.FindAction(promptAction));
        prompt.gameObject.SetActive(false);
        promptsToAdd.Add(promptObject);
        if (state == HelpPromptState.Deactivated) ModifyCurrentPrompts();
        // Else wait until it is deactivated to do the update
    }

    public void RemovePrompt(string promptName)
    {
        foreach (HelpPromptElement prompt in GetCurrentPrompts())
        {
            if (prompt.promptName == promptName)
                promptsToRemove.Add(prompt.gameObject);
        }
        if (state == HelpPromptState.Deactivated) ModifyCurrentPrompts();
    }

    private void ModifyCurrentPrompts()
    {
        foreach (GameObject prompt in promptsToAdd)
            prompt.SetActive(true);
        foreach (GameObject prompt in promptsToRemove)
            Destroy(prompt);
        promptsToAdd = new();
        promptsToRemove = new();
    }

    #endregion

    public HashSet<HelpPromptElement> GetCurrentPrompts()
    {
        HashSet<HelpPromptElement> currentPrompts = new();
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out HelpPromptElement prompt))
                currentPrompts.Add(prompt);
        }
        return currentPrompts;
    }

    public void RefreshVisuals()
    {
        lastUsedInputDevice = InputSystem.devices
            .Where(d => d.lastUpdateTime > 0)
            .OrderByDescending(d => d.lastUpdateTime)
            .FirstOrDefault();
        foreach (HelpPromptElement prompt in GetCurrentPrompts())
            prompt.RefreshVisuals();
    }

    #region Helper Functions

    #endregion
}

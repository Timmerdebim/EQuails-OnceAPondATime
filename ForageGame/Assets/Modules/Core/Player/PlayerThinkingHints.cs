using NPC;
using UnityEngine;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using TDK.ItemSystem;
using TDK.ItemSystem.Inventory;
using System.Collections;

[RequireComponent(typeof(DialogueBox))]
public class PlayerThinkingHints : MonoBehaviour
{
    [SerializeField] private DialogueBox thinkingBox;
    private CancellationTokenSource textCtxSource = new CancellationTokenSource();

    [Header("Wing Broken")]
    [SerializeField] private List<string> wingBrokenHints;
    [SerializeField] private int wingbrokenHintDuration = 1500;

    [Header("Not enough Energy")]
    [SerializeField] private List<string> noEnergyHints;
    [SerializeField] private int noEnergyHintDuration = 500;

    [Header("Inventory Full")]
    [SerializeField] private List<string> inventoryFullHints;
    [SerializeField] private int inventoryFullHintDuration = 1500;

    [Header("Sleeping Hints")]
    public SleepingHintDatabase sleepingHintDatabase;
    [SerializeField] private int sleepingHintDuration = 3000;
    
    void OnEnable()
    {
        StoryFlagManager.onStoryFlagsLoaded += ShowSleepingHint;
    }

    void OnDisable()
    {
        StoryFlagManager.onStoryFlagsLoaded -= ShowSleepingHint;
    }

    #region ctx token bollocks
    private void CancelCurrentToken()
    {
        if (textCtxSource != null && !textCtxSource.IsCancellationRequested)
        {
            textCtxSource.Cancel();
            textCtxSource.Dispose();
        }
    }

    private void ResetToken()
    {
        CancelCurrentToken();
        textCtxSource = new CancellationTokenSource();
    }
    #endregion

    #region API
    public void ShowWingBrokenThought()
    {
        int hintindex = UnityEngine.Random.Range(0, wingBrokenHints.Count);
        ShowThought(wingBrokenHints[hintindex], wingbrokenHintDuration);
    }

    public void ShowNoEnergyThought()
    {
        int hintindex = UnityEngine.Random.Range(0, noEnergyHints.Count);
        ShowThought(noEnergyHints[hintindex], noEnergyHintDuration);
    }

    public void ShowInventoryFullHint()
    {
        int hintindex = UnityEngine.Random.Range(0, inventoryFullHints.Count);
        ShowThought(inventoryFullHints[hintindex], inventoryFullHintDuration);
    }


    private List<SleepingHint> GetEligibleHints()
    {
        var flags = StoryFlagManager.Instance;
        var inventory = InventoryController.Instance;

        return sleepingHintDatabase.GetAllAssets()
                .Where(s =>
                    flags.FlagListActive(s.requiredFlags) &&
                    !flags.AnyFlagActive(s.absentFlags) &&
                    inventory.seenItems.IsSupersetOf(s.requiredItems) &&
                    !inventory.seenItems.Overlaps(s.absentItems)).ToList();
    }

    public void ShowSleepingHint()
    {
        StartCoroutine(ShowSleepingHintDelayed());
    }

    IEnumerator ShowSleepingHintDelayed()
    {
        yield return new WaitForSecondsRealtime(1f);

        var eligibleHints = GetEligibleHints();
        if(!eligibleHints.Any())
        {
            Debug.LogWarning($"[PlayerThinkingHints] No eligible hints to display?");
            yield break; //stop routine
        }
        int hintindex = UnityEngine.Random.Range(0, eligibleHints.Count);
        ShowThought(eligibleHints[hintindex].hint, sleepingHintDuration);
    }

    #endregion

    #region dialogue code
    private async Task ShowThought(string message, int duration, DialogueSpeakerType character = DialogueSpeakerType.WizardRock)
    {
        if(thinkingBox.dialogueOpen) 
        {
            Debug.LogWarning($"[PlayeerThinkingBoxController]: Dialogue box already open, canceling thought: {message}");
            return;
        }
        ResetToken();
        try
        {
            thinkingBox.OpenDialogue();
            await thinkingBox.SetText(message, character, textCtxSource.Token);

            await Task.Delay(duration, textCtxSource.Token);
        }
        catch (OperationCanceledException) { }
        finally
        {
            thinkingBox.CloseDialogue();
            CancelCurrentToken();
        }
    }
    
    #endregion
}

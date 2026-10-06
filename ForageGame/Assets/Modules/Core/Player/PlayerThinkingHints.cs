using NPC;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;

[RequireComponent(typeof(DialogueBox))]
public class PlayerThinkingHints : MonoBehaviour
{
    [SerializeField] private DialogueBox thinkingBox;
    [SerializeField] private float thoughtDuration = 1500f;
    private CancellationTokenSource textCtxSource = new CancellationTokenSource();

    [SerializeField] private List<String> wingBrokenHints;
    

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

    #region Data

    public void ShowWingBrokenThought()
    {
        int hintindex = UnityEngine.Random.Range(0, wingBrokenHints.Count);
        ShowThought(wingBrokenHints[hintindex]);
    }

    #endregion

    #region dialogue code
    private async Task ShowThought(string message, DialogueSpeakerType character = DialogueSpeakerType.WizardRock)
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

            await Task.Delay((int)thoughtDuration, textCtxSource.Token);
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

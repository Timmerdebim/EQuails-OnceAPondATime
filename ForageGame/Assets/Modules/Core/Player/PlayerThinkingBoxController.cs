using NPC;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

[RequireComponent(typeof(DialogueBox))]
public class PlayerThinkingBoxController : MonoBehaviour
{
    [SerializeField] private DialogueBox thinkingBox;
    [SerializeField] private float thoughtDuration = 1500f;
    private CancellationTokenSource textCtxSource = new CancellationTokenSource();
    

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

    #region actual code
    public async Task ShowThought(string message, DialogueSpeakerType character = DialogueSpeakerType.WizardRock)
    {
        ResetToken();
        if(thinkingBox.dialogueOpen) 
        {
            Debug.LogWarning($"[PlayeerThinkingBoxController]: Dialogue box already open, canceling thought: {message}");
            return;
        }
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

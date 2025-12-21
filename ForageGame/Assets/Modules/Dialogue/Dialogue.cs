using System;
using System.Threading;
using System.Threading.Tasks;
using Assets.Modules.Dialogue;
using Modules.Dialogue.DialogueDB;
using UnityEngine;

public class Dialogue : MonoBehaviour {
    [Header("References")]
    [SerializeField] private DialogueBox dialogueBox;
    public DialogueController dialogueController;
    [SerializeField] private DialogueBox.Character character;

    [Header("Settings")]
    [SerializeField] private float shortMessageDuration = 2000f;

    // State Tracking
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private CancellationTokenSource textCtxSource;
    private Task currentTypingTask;

    // Public getter
    public bool MessageRead { get; private set; } = false;

    private void Start() {
        textCtxSource = new CancellationTokenSource();
    }

    private void OnDestroy() {
        CancelCurrentToken();
    }

    public void SetDialogue(string[] text) {
        EndDialogue();
        MessageRead = false;
    }

    [ContextMenu("Next Message")]
    public async void Next() {
        if (isTyping) {
            CancelCurrentToken(); 
            return;
        }


        if (isDialogueActive && MessageRead) {
            EndDialogue();
            return;
        }

        ResetToken();

        DialogueLine line = dialogueController.GetNextDialogue(character.ToString());
        
        if (line == null) {
            EndDialogue();
            return;
        }
        
        if (line.StageID == "repeat") {
            MessageRead = true;
        }

        // 5. Open Dialogue Box if it's currently closed
        if (!isDialogueActive) {
            dialogueBox.OpenDialogue();
            isDialogueActive = true;
        }

        try {
            isTyping = true;
            
            string[] messageLines = line.Text.Split('\n');
            currentTypingTask = dialogueBox.SetText(messageLines, character, textCtxSource.Token);
            
            await currentTypingTask;
        }
        catch (OperationCanceledException) {
            // Task cancelled
        }
        finally {
            isTyping = false;
        }
    }

    public void WalkAway() {
        if (dialogueController == null) return;

        ResetToken();

        string textToDisplay = null; //very useful assignment of null value to uninitialized local variable, this one is new to me ~Lars

        if (isDialogueActive) {
            // Rude: Left while box was open
            textToDisplay = dialogueController.GetLeaveRudeDialogue(character.ToString())?.Text;
        } 
        else {
            // Polite: Left after closing the box
            textToDisplay = dialogueController.GetLeavePoliteDialogue(character.ToString())?.Text;
        }

        if (!string.IsNullOrEmpty(textToDisplay)) {
            _ = ShowShortMessage(textToDisplay);
        }
        else {
            CancelCurrentToken();
            EndDialogue();
        }
    }

    private async Task ShowShortMessage(string message) {
        try {
            if (!isDialogueActive) {
                dialogueBox.OpenDialogue();
                isDialogueActive = true;
            }

            isTyping = true;
            await dialogueBox.SetText(message, character, textCtxSource.Token);
            isTyping = false;

            await Task.Delay((int)shortMessageDuration, textCtxSource.Token);
        }
        catch (OperationCanceledException) { }
        finally {
            EndDialogue();
        }
    }

    private void EndDialogue() {
        dialogueBox.CloseDialogue();
        isDialogueActive = false;
        isTyping = false;

        CancelCurrentToken();
    }

    // --- Helpers ---

    private void CancelCurrentToken() {
        if (textCtxSource != null && !textCtxSource.IsCancellationRequested) {
            textCtxSource.Cancel();
            textCtxSource.Dispose();
        }
    }

    private void ResetToken() {
        CancelCurrentToken();
        textCtxSource = new CancellationTokenSource();
    }
}
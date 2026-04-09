using UnityEngine;
using Assets.Modules.Interaction;
using UnityEngine.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NPC
{
    public class NpcLocation : MonoBehaviour, IInteractable
    {
        //InteractablePrompt PopupPrompt; // UI element to prompt the player to interact

        [Header("Interaction Callbacks")]
        public UnityEvent onInteract; // Event to invoke when interacting
        public UnityEvent onFocus;
        public UnityEvent OnUnfocus;

        [Header("References")]
        [SerializeField] private DialogueBox dialogueBox;
        public DialogueController dialogueController; //TODO: replace

        [SerializeField] private NpcController npcController;

        [Header("Dialogue Display Settings")]
        [SerializeField] private float shortMessageDuration = 2000f;

        // State Tracking
        private bool isDialogueActive = false;
        private bool isTyping = false;
        private CancellationTokenSource textCtxSource;
        private Task currentTypingTask;

        // Public getter, TODO: unused publicly?
        public bool MessageRead { get; private set; } = false;

        private void Start()
        {
            //PopupPrompt = GetComponentInChildren<InteractablePrompt>(true);
            textCtxSource = new CancellationTokenSource();
        }

        private void OnDestroy() {
            CancelCurrentToken();
        }

        #region Interaction
        /// <summary>
        /// These are mostly unused now, they just call Next() and WalkAway()
        /// </summary>

        public virtual void Interact()
        {
            print("Interacting with " + gameObject.name);

            onInteract?.Invoke();
            GetComponentInChildren<Renderer>().material.color = Color.cyan;
            Next();
        }

        public virtual void Focus()
        {
            print("Focused on " + gameObject.name);
            onFocus?.Invoke();

            //PopupPrompt?.Activate();
        }

        public virtual void Unfocus()
        {
            print("Unfocused from " + gameObject.name);

            OnUnfocus?.Invoke();
            WalkAway();

            //PopupPrompt?.Deactivate();
        }
        #endregion
        #region Dialogue

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

            DialogueLine line = npcController.GetNextDialogue(this);
            Character character = Character.Bracken; //TODO: merge with DB

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
            Character character = Character.Bracken; //TODO: DEBUG

            if (isDialogueActive) {
                // Rude: Left while box was open
                //textToDisplay = dialogueController.GetLeaveRudeDialogue(character.ToString())?.Text;
            } 
            else {
                // Polite: Left after closing the box
                //textToDisplay = dialogueController.GetLeavePoliteDialogue(character.ToString())?.Text;
            }

            if (!string.IsNullOrEmpty(textToDisplay)) {
                _ = ShowShortMessage(textToDisplay, character);
            }
            else {
                CancelCurrentToken();
                EndDialogue();
            }
        }

        private async Task ShowShortMessage(string message, Character character) {
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
        #endregion
        // --- Helpers ---
        #region Cancellation Tokens
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
        #endregion
    }
}
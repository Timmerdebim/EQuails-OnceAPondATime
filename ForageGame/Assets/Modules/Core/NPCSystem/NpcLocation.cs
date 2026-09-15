using UnityEngine;
using UnityEngine.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
using TDK.InteractionSystem;

namespace NPC
{
    public class NpcLocation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DialogueBox dialogueBox;

        [SerializeField] private NpcController npcController;

        [SerializeField] private NpcLocationVisuals visuals;

        [Header("Dialogue Display Settings")]
        [SerializeField] private float shortMessageDuration = 2000f;

        // State Tracking
        private bool isDialogueActive = false;
        private bool isTyping = false;
        private CancellationTokenSource textCtxSource;
        private Task currentTypingTask;

        [SerializeField] private string lastTalkingEmotion; //to 'resume' the emotion if the player walked away mid-dialogue

        [SerializeField] private Interactable _interactable;


        //Public getter, TODO: unused publicly?
        public bool MessageRead = false;

        [SerializeField] private PopupTextbox statusIndicator;

        private bool isBeingDisabled = false; //insane stupid bullshit hack to not have the popup thing pop up when being disabled

        void Start()
        {
            textCtxSource = new CancellationTokenSource();
        }

        private void OnEnable()
        {
            _interactable.EnableInteraction();
            visuals.OnPopUp();
        }

        //THIS animation ALREADY DISABLES THE GAMEOBJECT
        public void ShrinkAway()
        {
            isBeingDisabled = true;
            HideStatusIndicator();
            _interactable.DisableInteraction();
            visuals.OnShrinkAway();
            isBeingDisabled = false; //yeah, you can see how much this SUCKS
        }

        public void ShowStatusIndicator()
        {
            Debug.LogError($"[NpcLocation: {gameObject.name}] me when I lie");
            statusIndicator.ShowTextbox(true);
        } 

        public void SetStatusIndicatorText(bool isMainDialogue)
        {
            if (isMainDialogue)
            {
                statusIndicator.SetTextColor(new Color(.75f, 0.1875f, 0.1875f));
                statusIndicator.SetText("!");
            }
            else
            {
                statusIndicator.SetTextColor(Color.white);
                statusIndicator.SetText("...");
            }
        }

        public void HideStatusIndicator() => statusIndicator.ShowTextbox(false);

        private void OnDestroy()
        {
            CancelCurrentToken();
        }

        #region Dialogue

        public void SetDialogue(string[] text)
        {
            EndDialogue();
            MessageRead = false;
        }

        public void SetEmotion(string emotion) => visuals.SetEmotion(emotion); //is a passthrough now

        [ContextMenu("Next Message")]
        public async void Next()
        {
            if (isTyping)
            {
                CancelCurrentToken();
                return;
            }


            if (isDialogueActive && MessageRead)
            {
                EndDialogue();
                npcController.OnDialogueFinished();
                lastTalkingEmotion = null; //so the NPC doesn't resume an emotion from a previous conversation
                return;
            }

            ResetToken();

            DialogueResult result = npcController.GetNextDialogue(this);
            MessageRead = result.CloseAfter;

            DialogueLine line = result.Line;

            if (line == null)
            {
                EndDialogue();
                return;
            }

            //Visual stuffs
            visuals.OnInteract();
            FaceTowardPlayer(); //by default, can get overriden by dialogue actions!
            if (!string.IsNullOrEmpty(line.emotion))
            {
                lastTalkingEmotion = line.emotion;
                SetEmotion(line.emotion);
            }
            else if (!string.IsNullOrEmpty(lastTalkingEmotion)) //resume conversation emotion
            {
                SetEmotion(lastTalkingEmotion);
            }


            // Dialogue Actions
            foreach (UnityEvent action in line.dialogueActions)
            {
                action.Invoke();
            }

            // 5. Open Dialogue Box if it's currently closed
            if (!isDialogueActive)
            {
                dialogueBox.OpenDialogue();
                HideStatusIndicator(); //hide the indicator
                isDialogueActive = true;
            }

            try
            {
                isTyping = true;

                string[] messageLines = line.Text.Split('\n'); //Okay so I absolutely fucking hate this, this means speech HAS to be done by the dialogue box itself ~Lars
                currentTypingTask = dialogueBox.SetText(messageLines, npcController.character, textCtxSource.Token);

                await currentTypingTask;
            }
            catch (OperationCanceledException)
            {
                // Task cancelled
            }
            finally
            {
                isTyping = false;
            }
        }


        //this fuck head is being called twice, which is why things even work in the first place

        //TODO: massive issue with leaving at the last line of a main dialogue. also showing status indicator afterward and making sure those don't clash
        public void WalkAway()
        {
            if (npcController == null || isBeingDisabled) return; //when the StoryStage changes and this location is no longer active, it will unfocus itself automatically. In this case prevent this function from running

            ResetToken();

            DialogueLine textToDisplay = null;

            //edge case: left on last dialogue box opened.
            //simply close the dialogue normally and evalutate the next stage.
            if (isDialogueActive && MessageRead)
            {
                Debug.LogWarning($"[NpcLocation: {gameObject.name}]: Nice try playtester, but walkaway edge case triggered, ending dialogue normally!");
                EndDialogue();
                npcController.OnDialogueFinished();
                lastTalkingEmotion = null; //so the NPC doesn't resume an emotion from a previous conversation
                return;
            }


            if (isDialogueActive)
            {
                //dialogue is actually done, neither rude or leave p
                // Rude: Left while box was open
                textToDisplay = npcController.GetLeaveRudeDialogue(this);
            }
            else
            {
                // Polite: Left after closing the box (or starting to do so)
                // now only actually gets a message if the regular stages are done ~Lars
                textToDisplay = npcController.GetLeavePoliteDialogue(this);
            }

            //Visual stuffs, play regardless of there actually being text to display
            visuals.OnInteract();
            ShowStatusIndicator(); //show it if we're not exiting stage 

            if (textToDisplay != null)
            {
                foreach (UnityEvent action in textToDisplay.dialogueActions)
                {
                    action.Invoke();
                }
                if (!string.IsNullOrEmpty(textToDisplay.emotion))
                {
                    SetEmotion(textToDisplay.emotion);
                }
                _ = ShowShortMessage(textToDisplay.Text, npcController.character);
            }
            else
            {
                CancelCurrentToken();
                EndDialogue();
            }
        }

        private async Task ShowShortMessage(string message, DialogueSpeakerType character)
        {
            try
            {
                if (!isDialogueActive)
                {
                    dialogueBox.OpenDialogue();
                    isDialogueActive = true;
                }

                isTyping = true;
                await dialogueBox.SetText(message, character, textCtxSource.Token);
                isTyping = false;

                await Task.Delay((int)shortMessageDuration, textCtxSource.Token);
            }
            catch (OperationCanceledException) { }
            finally
            {
                EndDialogue();
            }
        }

        private void EndDialogue()
        {
            dialogueBox.CloseDialogue();
            isDialogueActive = false;
            isTyping = false;

            //reset emotion after ending dialogue (i.e., close mouth)
            visuals.OnInteract();
            if (!string.IsNullOrEmpty(npcController.GetBaseEmotion(this))) SetEmotion(npcController.GetBaseEmotion(this));
            CancelCurrentToken();
        }
        #endregion

        #region DialogueActions

        public void AutoProgressStoryStage()
        {
            npcController.OnDialogueFinished();
        }



        public void FaceTowardPlayer() => visuals.FaceTowardPlayer();

        public void FaceAwayFromPlayer() => visuals.FaceAwayFromPlayer();

        public void FaceLeft() => visuals.FaceLeft();

        public void FaceRight() => visuals.FaceRight();

        #endregion
        // --- Helpers ---
        #region Cancellation Tokens
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
    }
}
using Assets.Modules.Dialogue.Typewriter_effect;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using System;

namespace NPC
{
    public class DialogueBox : MonoBehaviour
    {
        [SerializeField] Canvas canvas;
        [SerializeField] TextMeshProUGUI dialogueText;
        [SerializeField] AnimationCurve openCloseAnimation;
        [SerializeField] AnimationCurve newMessageAnimation;
        [SerializeField] float openCloseDuration;

        [SerializeField] public bool dialogueOpen = false;

        [Header("Positioning")]
        [SerializeField] private Vector3 rightPos;
        [SerializeField] private Vector3 leftPos;

        [Header("Audio / Speaking")]

        [SerializeField] GibberishSpeaker speaker;


        //CancellationTokenSource textCtxSource;
        CancellationTokenSource animationCtxSource;
        Task animateOut;
        Task animateIn;


        private void Start()
        {
            canvas.gameObject.SetActive(false);
            canvas.transform.localScale = Vector3.zero;
        }

        public async void OpenDialogue()
        {
            await CancelAnimations();
            speaker.DialogueOpened();
            animateIn = AnimateIn(animationCtxSource.Token);
        }

        public async void CloseDialogue()
        {
            await CancelAnimations();
            animateOut = AnimateOut(animationCtxSource.Token);
            speaker.DialogueClosed();
        }

        private async Task AnimateIn(CancellationToken ctx)
        {
            canvas.gameObject.SetActive(true);
            canvas.transform.localScale = Vector3.zero;

            float elapsedTime = 0f;
            while (elapsedTime < openCloseDuration)
            {
                if (ctx.IsCancellationRequested) break;

                float scale = openCloseAnimation.Evaluate(elapsedTime / openCloseDuration);
                canvas.transform.localScale = new Vector3(scale, scale, scale);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            canvas.transform.localScale = Vector3.one;
            dialogueOpen = true;
        }

        private async Task AnimateOut(CancellationToken ctx)
        {
            float elapsedTime = 0f;
            while (elapsedTime < openCloseDuration)
            {
                if (ctx.IsCancellationRequested) break;

                float scale = openCloseAnimation.Evaluate(1 - (elapsedTime / openCloseDuration));
                canvas.transform.localScale = new Vector3(scale, scale, scale);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }
            dialogueOpen = false;
            canvas.transform.localScale = Vector3.zero;
            canvas.gameObject.SetActive(false);
        }

        private async Task AnimateNewMessage(CancellationToken ctx)
        {
            float elapsedTime = 0f;
            float duration = newMessageAnimation[newMessageAnimation.length - 1].time;
            
            while (elapsedTime < duration)
            {
                if (ctx.IsCancellationRequested) break;

                float scale = newMessageAnimation.Evaluate(elapsedTime);
                canvas.transform.localScale = scale * Vector3.one;
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }
            canvas.transform.localScale = Vector3.one;
        }

        private async Task CancelAnimations()
        {
            animationCtxSource?.Cancel();

            if (animateIn?.IsCompleted == false) { await animateIn; }
            if (animateOut?.IsCompleted == false) { await animateOut; }

            animationCtxSource = new CancellationTokenSource();
        }

        public async Task SetText(string[] texts, DialogueSpeakerType character, CancellationToken ctx)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                if (ctx.IsCancellationRequested) return;
                await SetText(texts[i], character, ctx);

                if (i < texts.Length - 1)
                {
                    try
                    {
                        await Task.Delay(1000, ctx);
                    }
                    catch (TaskCanceledException)
                    {
                        return;
                    }
                }
            }
        }

        public async Task SetText(string text, DialogueSpeakerType speakerType, CancellationToken ctx)
        {
            await CancelAnimations();
            Task typewriting = dialogueText.TypewriteText(text, ctx);

            //start gibberish speech, by name is inefficient but who cares.
            speaker.Speak(text.Length);

            if (animateIn?.IsCompleted == false)
            {
                await animateIn;
            }
            AnimateNewMessage(ctx);

            await typewriting;
        }

        //Readables might be positioned in a way where the thinking box would clip into them if it is displayed on the right
        //this function is there to address it (keep in mind thinking box art is symmetrical, unlike speaking boxes)
        //yeah yeah, not pretty I know, this stuff just awkwark
        public void SetThinkingBoxOrientation(bool faceRight)
        {
            var basePos = transform.localPosition;
            var newXpos = faceRight ? Mathf.Abs(basePos.x): -Mathf.Abs(basePos.x);
            
            transform.localPosition = new Vector3(newXpos, basePos.y, basePos.z);
        }
    }
}
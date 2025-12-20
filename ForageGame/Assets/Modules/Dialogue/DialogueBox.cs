using Assets.Modules.Dialogue.Typewriter_effect;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Modules.Dialogue
{
    public class DialogueBox : MonoBehaviour
    {
        [SerializeField] Canvas canvas;
        [SerializeField] TextMeshProUGUI dialogueText;
        [SerializeField] AnimationCurve openCloseAnimation;
        [SerializeField] AnimationCurve newMessageAnimation;
        [SerializeField] float openCloseDuration;

        //CancellationTokenSource textCtxSource;
        CancellationTokenSource animationCtxSource;
        Task animateOut;
        Task animateIn;

        //FMOD stuff
        public FMODUnity.EventReference GibberishSpeechEvent;
        FMOD.Studio.EventInstance GibberishSpeech; 
        FMOD.Studio.PARAMETER_ID CharacterParameterId, SyllableCountParameterId; 

        public enum Character { Bracken, Mosswick, Grimble, Lyria }; 

        private void Start()
        {
            canvas.gameObject.SetActive(false);
            canvas.transform.localScale = Vector3.zero;
            
            GibberishSpeech = FMODUnity.RuntimeManager.CreateInstance(GibberishSpeechEvent);
            GibberishSpeech.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform.parent.gameObject)); //oof nasty hack to get the position right hehe
        }

        public async void OpenDialogue()
        {
            await CancelAnimations();
            animateIn = AnimateIn(animationCtxSource.Token);
        }

        public async void CloseDialogue()
        {
            await CancelAnimations();
            animateOut = AnimateOut(animationCtxSource.Token);
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
            GibberishSpeech.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            if (animateIn?.IsCompleted == false) { await animateIn; }
            if (animateOut?.IsCompleted == false) { await animateOut; }

            animationCtxSource = new CancellationTokenSource();
        }

        public async Task SetText(string[] texts, Character character, CancellationToken ctx)
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

        public async Task SetText(string text, Character character, CancellationToken ctx)
        {
            await CancelAnimations();
            Task typewriting = dialogueText.TypewriteText(text, ctx);

            //start gibberish speech, by name is inefficient but who cares.
            int syllables = math.clamp(text.Length / 5, 1, 10); //THIS IS A PLACEHOLDER, THIS SHOULD BE PART OF THE DIALOGUE SO ~Lars
            Debug.Log($"Speaking, {syllables} Syllables!");
            GibberishSpeech.setParameterByName("Syllable Count", syllables);
            GibberishSpeech.setParameterByName("Character", (int) character);
            GibberishSpeech.start();

            if (animateIn?.IsCompleted == false)
            {
                await animateIn;
            }
            AnimateNewMessage(ctx);

            await typewriting;
        }
    }
}
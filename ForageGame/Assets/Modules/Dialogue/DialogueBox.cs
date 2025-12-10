using Assets.Modules.Dialogue.Typewriter_effect;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
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
        FMOD.Studio.EventInstance GibberishSpeech; //instance of the previous
        FMOD.Studio.PARAMETER_ID CharacterParameterId, SyllableCountParameterId; //parameters of previous

        private void Start()
        {
            canvas.gameObject.SetActive(false);
            canvas.transform.localScale = Vector3.zero;
            //Create the FMOD event
            GibberishSpeech = FMODUnity.RuntimeManager.CreateInstance(GibberishSpeechEvent);
            GibberishSpeech.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
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
                if (ctx.IsCancellationRequested)
                {
                    break;
                }

                float scale = openCloseAnimation.Evaluate(elapsedTime / openCloseDuration);
                canvas.transform.localScale = new Vector3(scale, scale, scale);
                elapsedTime += Time.deltaTime;
                await System.Threading.Tasks.Task.Yield();
            }

            canvas.transform.localScale = Vector3.one;
        }

        private async Task AnimateOut(CancellationToken ctx)
        {
            float elapsedTime = 0f;
            while (elapsedTime < openCloseDuration)
            {
                if(ctx.IsCancellationRequested)
                {
                    break;
                }

                float scale = openCloseAnimation.Evaluate(1 - (elapsedTime / openCloseDuration));
                canvas.transform.localScale = new Vector3(scale, scale, scale);
                elapsedTime += Time.deltaTime;
                await System.Threading.Tasks.Task.Yield();
            }
            canvas.transform.localScale = Vector3.zero;
            canvas.gameObject.SetActive(false);
        }

        private async void AnimateNewMessage(CancellationToken ctx)
        {
            float elapsedTime = 0f;
            while (elapsedTime < newMessageAnimation[newMessageAnimation.length-1].time)
            {
                if (ctx.IsCancellationRequested)
                {
                    break;
                }

                float scale = newMessageAnimation.Evaluate(elapsedTime);
                canvas.transform.localScale = scale * Vector3.one;
                elapsedTime += Time.deltaTime;
                await System.Threading.Tasks.Task.Yield();
            }
            canvas.transform.localScale = Vector3.one;
        }

        private async Task CancelAnimations()
        {
            //textCtxSource?.Cancel();
            //textCtxSource?.Token.WaitHandle.WaitOne();
            //textCtxSource = new CancellationTokenSource();

            animationCtxSource?.Cancel();

            //stop any ongoing speech
            GibberishSpeech.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            if (animateIn?.IsCompleted == false) { await animateIn; }
            if (animateOut?.IsCompleted == false) { await animateOut; }

            animationCtxSource = new CancellationTokenSource();
        }

        //what actually draws characters on screen
        public async Task SetText(string text, CancellationToken ctx)
        {
            await CancelAnimations();
            Task typewriting = dialogueText.TypewriteText(text, ctx);

            //start gibberish speech
            GibberishSpeech.setParameterByName("Syllable Count", 6);
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
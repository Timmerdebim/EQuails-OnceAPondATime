using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Modules.Dialogue.Typewriter_effect
{
    public static class TypewriterEffect
    {
        public static async Task TypewriteText(this TextMeshProUGUI textbox, string message, CancellationToken ctx, bool underscore = true, float typeDelay = 0.03f, float clickSpeedMultiplication = 2f)
        {
            textbox.text = "";
            string text = "";

            for (int i = 0; i < message.Length; ++i)
            {
                if (ctx.IsCancellationRequested)
                {
                    textbox.text = message;
                    return;
                }

                text += message[i];
                string append = (underscore && i < message.Length - 1) ? "_" : "";
                textbox.text = text + append;

                float delay = Input.anyKey ? typeDelay / clickSpeedMultiplication : typeDelay;
                await System.Threading.Tasks.Task.Delay((int)(delay * 1000));
            }
        }
    }
}
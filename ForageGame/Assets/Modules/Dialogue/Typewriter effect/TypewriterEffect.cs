using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Modules.Dialogue.Typewriter_effect
{
    public static class TypewriterEffect
    {
        public static async void TypewriteText(this TextMeshProUGUI textbox, string message, bool underscore = true, float typeDelay = 0.05f, float clickSpeedMultiplication = 3f)
        {
            textbox.text = "";
            string text = "";

            for (int i = 0; i < message.Length; ++i)
            {
                text += message[i];
                string append = (underscore && i < message.Length - 1) ? "_" : "";
                textbox.text = text + append;

                float delay = Input.anyKey ? typeDelay / clickSpeedMultiplication : typeDelay;
                await System.Threading.Tasks.Task.Delay((int)(delay * 1000));
            }
        }
    }
}
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Modules.Dialogue.Typewriter_effect
{
    public class TypewriterTMProTextbox : TextMeshProUGUI
    {
        [SerializeField] string message;
        [SerializeField] float typeDelay = 0.05f;
        [SerializeField] float clickSpeedMultiplication = 3f;
        [Tooltip("If  true, displays an underscore after the text to show that it's being typed")][SerializeField] bool underscore;
        [SerializeField] AudioSource typingSound;
        [SerializeField] bool playOnStart = true;

        protected override void Start()
        {
            base.Start();
            if (playOnStart) TypeText();
        }

        public async void TypeText()
        {
            this.text = "";
            string text = "";

            for (int i = 0; i < message.Length; ++i)
            {
                text += message[i];
                string append = (underscore && i < message.Length - 1) ? "_" : "";
                this.text = text + append;

                if (typingSound != null)
                {
                    typingSound.Play();
                }

                float delay = Input.anyKey ? typeDelay / clickSpeedMultiplication : typeDelay;
                await System.Threading.Tasks.Task.Delay((int)(delay * 1000));
            }
        }

    }
}
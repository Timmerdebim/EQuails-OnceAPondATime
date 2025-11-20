using Assets.Modules.Dialogue;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Dialogue : MonoBehaviour
{

    // Idea to fix dialogue: integrate Dialogue with DialogueBox. For speeding up, make a StartSpeedup and StopSpeedup function, start on click button, stop on release. 
    // When we get an input, first cancel typewrite animation, or if it is already finished, go to next text


    [SerializeField]string[] messages = {
        "Hello, adventurer!",
        "Welcome to our village.",
        "Feel free to explore and talk to the locals.",
        "Good luck on your journey!"
    };

    int index = 0;

    [SerializeField] DialogueBox dialogueBox;

    Task textWriting;
    CancellationTokenSource textCtxSource = new CancellationTokenSource();

    [ContextMenu("Start Dialogue")]
    private void StartDialogue()
    {
        dialogueBox.OpenDialogue();
        textWriting = dialogueBox.SetText(messages[index], textCtxSource.Token);
        index++;
    }

    private void EndDialogue()
    {
        dialogueBox.CloseDialogue();
        index = 0;
    }

    [ContextMenu("Next Message")]
    public void Next()
    {
        print("Next Message");
        if(index <= 0)
        {
            StartDialogue();
        }
        else
        {
            if(textWriting != null && !textWriting.IsCompleted)
            {
                textCtxSource.Cancel();
                textCtxSource = new CancellationTokenSource();
                return;
            }
            
            index++;
            
            if (index >= messages.Length)
            {
                EndDialogue();
                return;
            }

            textWriting = dialogueBox.SetText(messages[index], textCtxSource.Token);
        }

    }
}

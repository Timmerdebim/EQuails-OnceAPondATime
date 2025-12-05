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

    [SerializeField] string normalLeaveMessage = null;
    [SerializeField] string rudeLeaveMessage = null;

    int index = 0;

    [SerializeField] DialogueBox dialogueBox;

    Task textWriting;

    CancellationTokenSource textCtxSource = new CancellationTokenSource();

    private void EndDialogue()
    {
        dialogueBox.CloseDialogue();
        index = 0;
    }

    private Task Speak(string message)
    {
        if(index <= 0)
        {
            dialogueBox.OpenDialogue();
        }

        return dialogueBox.SetText(message, textCtxSource.Token);
    }

    [ContextMenu("Next Message")]
    public async void Next()
    {
        print("Next Message");

        if(textWriting != null && !textWriting.IsCompleted)
        {
            textCtxSource.Cancel();
            if(textWriting != null && !textWriting.IsCompleted)
            {
                await textWriting;
            }
            textCtxSource = new CancellationTokenSource();
            return;
        }

        if (index > messages.Length - 1)
        {
            EndDialogue();
            return;
        }

        textWriting = Speak(messages[index]);

        index++;
    }

    public void WalkAway()
    {
        if(index <= 0)
        {
            //This means we are not in conversation
            if(normalLeaveMessage != null)
            {
                textWriting = ShortMessage(normalLeaveMessage);
            }
        }
        else
        {
            //This means we are in conversation, so it's rude to leave
            if(rudeLeaveMessage != null)
            {
                textWriting = ShortMessage(rudeLeaveMessage);
            }
        }
    }

    private async Task ShortMessage(string message)
    {
        // Called when the player rudely walks away from the conversation

        await Speak(message);

        float timeWaited = 0;
        while (!textCtxSource.IsCancellationRequested && timeWaited < 3000)
        {
            await Task.Delay(10);
            timeWaited += 10;
        }
        EndDialogue();
    }
}

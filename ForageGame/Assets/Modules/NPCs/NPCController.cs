using System.Collections.Generic;
using Assets.Modules.Interaction;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Interactions;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcLabel; // Frog/Rat/Squirrel/Bird
    [SerializeField] private NPCStoryState[] storyStates; // ORDERED SEQUENTIALLY
    [SerializeField] private int storyIndex;
    private DialogueBlock currentDialogueBlock;
    [SerializeField] private Dialogue dialogue;

    void Start() // start runs after the stuff has been loaded? hopefully?
    {
        if (storyStates.Length == 0)
        {
            Debug.Log("NPC: ERROR: dialogueStates is empty.");
            return;
        }
    }

    private bool CheckFlags(StoryFlag[] flags)
    {
        foreach (StoryFlag flag in flags)
        {
            if (StoryManager.Instance.Get(flag))
                return false;
        }
        return true;
    }

    private void SetFlags(StoryFlag[] flags, bool value)
    {
        foreach (StoryFlag flag in flags)
            StoryManager.Instance.Set(flag, value);
    }

    private void TakeQuestItem()
    {
        // If need nothing, return
        if (storyStates[storyIndex].RequiredItems.Length == 0) return;
        // Else try and get first match
        for (int i = 0; i < storyStates[storyIndex].RequiredItems.Length; i++)
        {
            if (InventorySystem.Instance.GiveItem(storyStates[storyIndex].RequiredItems[i].item))
            {
                StoryManager.Instance.Set(storyStates[storyIndex].RequiredItems[i].flag, true);
                return;
            }
        }
    }

    private void SelectNextBlock()
    {
        // We try to advance the quest
        if (CheckFlags(storyStates[storyIndex].QuestBlock.requiredFlags))
        {
            SetBlock(storyStates[storyIndex].QuestBlock);
            storyIndex += 1;
            return;
        }

        // We get the next block from the current quest state
        foreach (DialogueBlock dialogueBlock in storyStates[storyIndex].DialogueBlocks)
        {
            if (CheckFlags(dialogueBlock.requiredFlags))
            {
                SetBlock(dialogueBlock);
                return;
            }
        }

        // If all else fails; set to default
        SetBlock(storyStates[storyIndex].DefaultBlock);
    }

    private void SetBlock(DialogueBlock dialogueBlock)
    {
        // TODO
        currentDialogueBlock = dialogueBlock;
    }

    // ------------ INTERACTION SYSTEM ------------

    public void Focus()
    {
        throw new System.NotImplementedException();
    }

    public void Interact(UnityAction StopInteractionCallback)
    {
        // if done, set new dialogue block
        if (dialogue._messageRead)
        {
            TakeQuestItem(); // try and take a requested item
            SelectNextBlock(); // set the next dialog block
            SetFlags(currentDialogueBlock.flagsToSet, true); // trigger their actativation flags
        }
        // run dialogue
        dialogue.Next();
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }

    public void Unfocus()
    {
        throw new System.NotImplementedException();
    }
}

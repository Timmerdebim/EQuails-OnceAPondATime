using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Story State", menuName = "Dialogue System/NPC Story State")]
public class NPCStoryState : ScriptableObject
{
    [SerializeField] private ItemFlag[] requiredItems;
    public ItemFlag[] RequiredItems => requiredItems;
    [SerializeField] private DialogueBlock questBlock;
    public DialogueBlock QuestBlock => questBlock;
    [SerializeField] private DialogueBlock defaultBlock;
    public DialogueBlock DefaultBlock => defaultBlock;
    [SerializeField] private DialogueBlock[] dialogueBlocks; // in order of which they are checked (first come first serve)
    public DialogueBlock[] DialogueBlocks => dialogueBlocks;
}

public struct ItemFlag
{
    [SerializeField] public Item item;
    [SerializeField] public StoryFlag flag;
}

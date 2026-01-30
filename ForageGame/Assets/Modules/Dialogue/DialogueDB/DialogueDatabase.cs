using System;
using System.Collections.Generic;
using System.Linq;

namespace Modules.Dialogue.DialogueDB
{
    [Serializable]
    public class DialogueDatabase
    {
        public List<CharacterProfile> Characters = new List<CharacterProfile>();
        public CharacterProfile GetCharacter(string name) => 
            Characters.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    [Serializable]
    public class CharacterProfile
    {
        public string Name;
        public List<DialogueBlock> Blocks = new List<DialogueBlock>();
    }

    [Serializable]
    public class DialogueBlock
    {
        public string BlockID => RequiredFlags.Count == 0 ? "default" : string.Join("+", RequiredFlags);
        public List<StoryFlag> RequiredFlags = new List<StoryFlag>();
        public List<DialogueLine> Lines = new List<DialogueLine>();

        // --- Helpers for specific line types ---
        
        // The numeric story lines (0, 1, 2...)
        public List<DialogueLine> StandardLines => Lines.Where(l => l.IsStoryStage).ToList();
        
        // Helper to check if this block contains a specific special stage
        public DialogueLine GetSpecialLine(string stageID)
        {
            return Lines.FirstOrDefault(l => l.StageID.Equals(stageID, StringComparison.OrdinalIgnoreCase));
        }
    }

    [Serializable]
    public class DialogueLine
    {
        public string StageID; 
        public string Text;
        
        // Returns true if the stage is NOT one of our special keywords
        public bool IsStoryStage 
        {
            get 
            {
                // If it parses as a number, it's a story stage. 
                // Alternatively, just check against known keywords.
                return int.TryParse(StageID, out _);
            }
        }
    }

    // --- RUNTIME STATE (Save Data) ---
    [Serializable]
    public class DialogueRuntimeState
    {
        public Dictionary<string, CharacterState> CharacterStates = new Dictionary<string, CharacterState>();

        public CharacterState GetOrAddState(string charName)
        {
            if (!CharacterStates.ContainsKey(charName))
                CharacterStates[charName] = new CharacterState();
            return CharacterStates[charName];
        }
    }

    [Serializable]
    public class CharacterState
    {
        public string CurrentBlockID;
        public int CurrentLineIndex;
        public HashSet<string> CompletedBlocks = new HashSet<string>();
    }
}
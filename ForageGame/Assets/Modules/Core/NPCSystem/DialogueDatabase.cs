using System;
using System.Collections.Generic;
using System.Linq;

namespace NPC
{
    /// <summary>
    /// top-level container, might host some helper funcs later
    /// </summary>
    [Serializable]
    public class DialogueDatabase
    {
        public List<StoryStage> storyStages = new List<StoryStage>();
    }

    /// <summary>
    /// The stage of progression in the story at any one point.
    /// Holds the dialogue of multiple active NpcLocations at once, though not all of them may progress the story (only main lines do).
    /// Thus, can be viewed as an 'active state' as such.
    /// </summary>
    [Serializable]
    public class StoryStage
    {
        public string BlockID => RequiredFlags.Count == 0 ? "default" : string.Join("+", RequiredFlags); //TODO: no bueno
        public List<StoryFlag> RequiredFlags = new List<StoryFlag>();
        public List<string> SetFlags = new List<string>();
        public List<LocationDialogue> locationDialogues = new List<LocationDialogue>();

    }

    /// <summary>
    /// Holds the dialoge for a given NpcLocation
    /// </summary>
    [Serializable]
    public class LocationDialogue
    {
        //Flavor dialogue does not progress StoryStage. Use for puzzle hints indeed flavor text
        public bool isFlavorDialogue = false;
        public List<DialogueLine> Lines = new List<DialogueLine>();

        // --- Helpers for specific line types ---
        //TODO: Lars no likey

        // The numeric story lines (0, 1, 2...)
        public List<DialogueLine> StandardLines => Lines.Where(l => l.IsStoryStage).ToList();
        public DialogueLine GetSpecialLine(string stageID) //TODO: THIS IS HOW WE GET LEAVE MESSAGES AND STUFF! SUCKS!
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
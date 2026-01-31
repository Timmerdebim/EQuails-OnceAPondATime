using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Modules.Dialogue.DialogueDB
{
    public class DialogueController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private List<TextAsset> _sourceFiles;
        
        private DialogueDatabase _database;
        private DialogueRuntimeState _state; 
        private HashSet<string> _activeFlags = new HashSet<string>();

        //Changed to Start() from Awake() since it gave inconsistent behavior in terms of timing ~Lars
        private void Start()
        {
            List<string> rawText = _sourceFiles.Select(f => f.text).ToList();
            _database = DialogueParser.Parse(rawText);
            _state = new DialogueRuntimeState();
        }

        // --- FLAG MANAGEMENT ---
        public void SetFlag(string flag) => _activeFlags.Add(flag);
        public void RemoveFlag(string flag) => _activeFlags.Remove(flag);

        // --- DIALOGUE FLOW ---

        public DialogueLine GetNextDialogue(string charName)
        {
            CharacterProfile character = _database.GetCharacter(charName);
            if (character == null) return null;

            CharacterState charState = _state.GetOrAddState(charName);

            // 1. CONTINUE CURRENT BLOCK
            if (!string.IsNullOrEmpty(charState.CurrentBlockID))
            {
                var line = GetNextLineInBlock(character, charState);
                if (line != null) return line;
            }

            // 2. FIND NEW UNREAD CONTENT (Catch-up logic)
            // Checks Inventory Flags to see what is *possible* to play.
            var nextNewBlock = character.Blocks
                .Where(b => IsBlockValidByFlags(b) && !charState.CompletedBlocks.Contains(b.BlockID))
                .OrderBy(b => b.RequiredFlags.Count) // Play simplest first (Catch-up)
                .FirstOrDefault();

            if (nextNewBlock != null)
            {
                StartBlock(charState, nextNewBlock);
                return GetNextLineInBlock(character, charState);
            }

            // 3. FALLBACK (Repeat)
            // Uses History Logic (Only repeats what we have actually talked about)
            return GetMostRelevantLine(character, "repeat");
        }

        // --- LEAVE METHODS ---

        public DialogueLine GetLeaveRudeDialogue(string charName)
        {
            return GetMostRelevantLine(_database.GetCharacter(charName), "leave_rude");
        }

        public DialogueLine GetLeavePoliteDialogue(string charName)
        {
            return GetMostRelevantLine(_database.GetCharacter(charName), "leave_polite");
        }

        // --- CORE HELPERS ---

        /// <summary>
        /// Finds the most complex block that the user has ALREADY EXPERIENCED.
        /// </summary>
        private DialogueLine GetMostRelevantLine(CharacterProfile character, string specialStageID)
        {
            if (character == null) return null;
            CharacterState state = _state.GetOrAddState(character.Name);

            // Logic:
            // 1. Filter blocks to only those we have "Listened To" (Active, Completed, or Base).
            // 2. Sort by DESCENDING complexity (Show the most advanced topic we have discussed).
            // 3. Find the first one with the requested stage.

            var bestBlock = character.Blocks
                .Where(b => IsBlockListenedTo(b, state)) 
                .OrderByDescending(b => b.RequiredFlags.Count)
                .FirstOrDefault(b => b.GetSpecialLine(specialStageID) != null);

            return bestBlock?.GetSpecialLine(specialStageID);
        }

        /// <summary>
        /// Determines if a block is part of the player's conversation history.
        /// </summary>
        private bool IsBlockListenedTo(DialogueBlock block, CharacterState state)
        {
            // Case 1: It is the base layer (always valid context)
            if (block.RequiredFlags.Count == 0) return true;

            // Case 2: We are literally talking about it right now
            if (state.CurrentBlockID == block.BlockID) return true;

            // Case 3: We have finished talking about it in the past
            if (state.CompletedBlocks.Contains(block.BlockID)) return true;

            return false;
        }

        /// <summary>
        /// Determines if the player has the inventory required to START this block.
        /// </summary>
        private bool IsBlockValidByFlags(DialogueBlock block)
        {
            //return block.RequiredFlags.All(f => _activeFlags.Contains(f));
            return StoryFlagManager.Instance.FlagListActive(block.RequiredFlags);
        }

        private void StartBlock(CharacterState state, DialogueBlock block)
        {
            state.CurrentBlockID = block.BlockID;
            state.CurrentLineIndex = 0;
        }

        private DialogueLine GetNextLineInBlock(CharacterProfile character, CharacterState state)
        {
            var block = character.Blocks.FirstOrDefault(b => b.BlockID == state.CurrentBlockID);
            
            if (block == null || state.CurrentLineIndex >= block.StandardLines.Count)
            {
                if (block != null) state.CompletedBlocks.Add(block.BlockID);
                state.CurrentBlockID = null;
                return null;
            }

            var line = block.StandardLines[state.CurrentLineIndex];
            state.CurrentLineIndex++;

            // If we just finished the last line, mark complete immediately
            if (state.CurrentLineIndex >= block.StandardLines.Count)
            {
                state.CompletedBlocks.Add(block.BlockID);
                state.CurrentBlockID = null;
            }

            return line;
        }
    }
}
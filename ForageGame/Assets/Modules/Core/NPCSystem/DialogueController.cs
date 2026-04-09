using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;

namespace NPC
{
    public class DialogueController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private List<TextAsset> _sourceFiles;
        
        private DialogueDatabase _database;
        private DialogueRuntimeState _state; 

        //Changed to Start() from Awake() since it gave inconsistent behavior in terms of timing ~Lars
        private void Start()
        {
            List<string> rawText = _sourceFiles.Select(f => f.text).ToList();
            _database = DialogueParser.Parse(rawText);
            _state = new DialogueRuntimeState();
        }

        // --- DIALOGUE FLOW ---

        public DialogueLine GetNextDialogue()
        {
            //CharacterProfile character = _database.GetCharacter(charName);
            if (_database == null) return null;

            CharacterState charState = _state.GetOrAddState("Bracken");//TODO: fucked

            var stage = _database.storyStages[0]; //TODO: this is of course bs

            // 1. CONTINUE CURRENT BLOCK
            if (!string.IsNullOrEmpty(charState.CurrentBlockID))
            {
                var line = GetNextLineInBlock(stage, charState);
                if (line != null) return line;
            }

            // 2. FIND NEW UNREAD CONTENT (Catch-up logic)
            // Checks Inventory Flags to see what is *possible* to play.
            var nextNewBlock = _database.storyStages
                .Where(b => IsBlockValidByFlags(b) && !charState.CompletedBlocks.Contains(b.BlockID))
                .OrderBy(b => b.RequiredFlags.Count) // Play simplest first (Catch-up)
                .FirstOrDefault();

            if (nextNewBlock != null)
            {
                StartBlock(charState, nextNewBlock);
                return GetNextLineInBlock(nextNewBlock, charState);
            }

            // 3. FALLBACK (Repeat)
            // Uses History Logic (Only repeats what we have actually talked about)
            return GetMostRelevantLine(_database, "repeat");
        }

        // --- LEAVE METHODS ---

        public DialogueLine GetLeaveRudeDialogue(string charName)
        {
            return GetMostRelevantLine(_database, "leave_rude");
        }

        public DialogueLine GetLeavePoliteDialogue(string charName)
        {
            return GetMostRelevantLine(_database, "leave_polite");
        }

        // --- CORE HELPERS ---

        /// <summary>
        /// Finds the most complex block that the user has ALREADY EXPERIENCED.
        /// </summary>
        private DialogueLine GetMostRelevantLine(DialogueDatabase db, string specialStageID)
        {
            if (db == null) return null;
            CharacterState state = _state.GetOrAddState("Bracken"); //db.Name); //TODO: make it shut up

            // Logic:
            // 1. Filter blocks to only those we have "Listened To" (Active, Completed, or Base).
            // 2. Sort by DESCENDING complexity (Show the most advanced topic we have discussed).
            // 3. Find the first one with the requested stage.

            var bestBlock = db.storyStages
                .Where(b => IsBlockListenedTo(b, state)) 
                .OrderByDescending(b => b.RequiredFlags.Count)
                .FirstOrDefault(); //TODO: FUCKED NOW
                //.FirstOrDefault(b => b.GetSpecialLine(specialStageID) != null);

            return bestBlock?.locationDialogues[0].GetSpecialLine(specialStageID); //TODO: FUCKED NOW
        }

        /// <summary>
        /// Determines if a block is part of the player's conversation history.
        /// </summary>
        private bool IsBlockListenedTo(StoryStage block, CharacterState state)
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
        private bool IsBlockValidByFlags(StoryStage block)
        {
            return StoryFlagManager.Instance.FlagListActive(block.RequiredFlags);
        }

        private void StartBlock(CharacterState state, StoryStage block)
        {
            state.CurrentBlockID = block.BlockID;
            state.CurrentLineIndex = 0;
        }

        private DialogueLine GetNextLineInBlock(StoryStage stage, CharacterState state)
        {
            var block = stage;//.Blocks.FirstOrDefault(b => b.BlockID == state.CurrentBlockID);
            var locDialog = stage.locationDialogues[0]; //TODO: this entire var is bs
            
            if (block == null || state.CurrentLineIndex >= locDialog.StandardLines.Count)
            {
                if (block != null) state.CompletedBlocks.Add(block.BlockID);
                state.CurrentBlockID = null;
                return null;
            }

            var line = locDialog.StandardLines[state.CurrentLineIndex];
            state.CurrentLineIndex++;
            if (state.CurrentLineIndex >= locDialog.StandardLines.Count)
            {
                state.CompletedBlocks.Add(block.BlockID);
                state.CurrentBlockID = null;

                // Apply the Set-Flags
                foreach (string flagID in block.SetFlags)
                {
                    // NOTE: Replace 'SetFlag' with whatever method your StoryFlagManager uses 
                    // to add/activate a flag (e.g., AddFlag, ActivateFlag, SetBool, etc.)
                    StoryFlagManager.Instance.AddFlag(new StoryFlag(flagID));
                    
                }
            }

            // If we just finished the last line, mark complete immediately
            if (state.CurrentLineIndex >= locDialog.StandardLines.Count)
            {
                state.CompletedBlocks.Add(block.BlockID);
                state.CurrentBlockID = null;
            }

            return line;
        }
    }
}